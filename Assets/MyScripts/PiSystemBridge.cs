using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PiSystemBridge : MonoBehaviour
{
    const int MenuRi = 0;
    const int MenuData = 1;
    const int MenuMapStyle = 2;
    const int MenuRiver = 3;
    const int MenuCount = 4;

    public enum ControlMode
    {
        MapSelect,
        MenuSelect
    }

    [Header("Network")]
    public int listenPort = 5005;

    [Header("Existing Systems")]
    public MapSwitcher mapSwitcher;
    public WaterSystemManager waterSystemManager;

    [Header("Map Root For Zoom")]
    public Transform mapRoot;
    public float minScale = 0.8f;
    public float maxScale = 3.0f;

    [Header("Slider Visual")]
    public Transform sliderXContainer;
    public Transform sliderYContainer;
    public Transform intersectionMarker;

    [Header("Slider Movement Range (Local)")]
    public float sliderXMin = -2f;
    public float sliderXMax = 2f;
    public float sliderYMin = -1.5f;
    public float sliderYMax = 1.5f;

    [Header("River Highlight References")]
    public RiverHighlighter sgLangat;
    public RiverHighlighter sgLui;
    public RiverHighlighter sgBeranang;
    public RiverHighlighter sgRinching;
    public RiverHighlighter sgBtgNilai;
    public RiverHighlighter sgSemenyih;
    public RiverHighlighter sgLabu;
    public RiverHighlighter sgJijan;

    [Header("Menu Panels (ENC2 切换)")]
    public GameObject classPanel;
    public GameObject dataPanel;
    public GameObject datePanel;
    public GameObject parameterPanel;

    [Header("River Selection")]
    public float riverSelectionCooldown = 0.15f;
    public float snapRadius = 0.32f;

    [Header("Joystick / Gesture")]
    public float joyMoveCooldown = 0.12f;

    [Header("Zoom")]
    public float zoomCooldown = 0.08f;
    public float rotateStepDegrees = 6f;

    [Header("Menu Focus")]
    public Color menuFocusColor = new Color(1f, 0.55f, 0.1f, 1f);
    public float menuFocusPadding = 3f;
    public float menuFocusThickness = 1.5f;

    private float lastZoomTime = -10f;

    [Header("Dropdown 交互")]
    public bool dropdownOpenAndPickMode = true;
    public float dropdownStepCooldown = 0.12f;

    [Header("Debug State")]
    public ControlMode currentMode = ControlMode.MapSelect;
    public int currentMapIndex = 0;
    public int currentMenuIndex = 0; // 0 RI, 1 DATA, 2 MAPSTYLE, 3 RIVER
    public string currentRiver = "";

    private UdpClient udpClient;
    private IPEndPoint remoteEP;

    private int currentSlider1Raw = 0;
    private int currentSlider2Raw = 0;
    private float lastRiverSelectTime = -10f;
    private float lastJoyMoveTime = -10f;

    private readonly string[] menuNames = { "RI", "DATA", "MAPSTYLE", "RIVER" };
    private readonly List<Selectable> mapStyleSelectables = new List<Selectable>();
    private readonly List<Selectable> riverMenuSelectables = new List<Selectable>();
    private readonly RectTransform[] menuFocusFrames = new RectTransform[MenuCount];

    // 只在地图阶段 hover，高亮但不提交
    private string hoverRiver = "";
    private string selectedRiver = "";

    // 当前 panel 内 UI 选择位置
    private int classUiIndex = 0;
    private int dataUiIndex = 0;
    private int mapStyleUiIndex = 0;
    private int riverUiIndex = 0;
    private int dateUiIndex = 0;
    private int parameterUiIndex = 0;
    private int mergedDataRow = 0;
    private bool enc3ZoomMode = true;

    // Dropdown 展开状态
    private bool dropdownOpen = false;
    private TMP_Dropdown openTmpDropdown = null;
    private Dropdown openLegacyDropdown = null;
    private float nextDropdownStepTime = 0f;

    struct RiverSelectionSnapshot
    {
        public int classIndex;
        public int dataToggleIndex;
        public int dataSlotIndex;
        public int parameterIndex;
        public string startMonth;
        public string startYear;
        public string durationValue;
        public bool useMonthDuration;
        public bool hadParameterSelection;
    }

    void Start()
    {
        StartReceiver();
        RefreshMenuReferences();
        SyncLocalStateFromUI();
        ApplyMapIndex();
        ApplyMenuPanel();
        Debug.Log("[PiSystemBridge] Started.");
    }

    void OnEnable()
    {
        if (udpClient == null)
            StartReceiver();

        RefreshMenuReferences();
        SyncLocalStateFromUI();
        ApplyMenuPanel();
    }

    void OnDisable()
    {
        StopReceiver();
    }

    void OnApplicationQuit()
    {
        StopReceiver();
    }

    void Update()
    {
        PumpUdpMessages();
    }

    void StartReceiver()
    {
        try
        {
            remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
            udpClient = new UdpClient(listenPort);
            udpClient.Client.ReceiveTimeout = 1;
            udpClient.Client.Blocking = false;
            Debug.Log("[PiSystemBridge] UDP listening on port " + listenPort);
        }
        catch (Exception e)
        {
            Debug.LogError("[PiSystemBridge] Failed to start UDP receiver: " + e.Message);
        }
    }

    void StopReceiver()
    {
        try
        {
            if (udpClient != null)
            {
                udpClient.Close();
                udpClient = null;
            }
        }
        catch
        {
        }
    }

    void PumpUdpMessages()
    {
        if (udpClient == null) return;

        try
        {
            while (udpClient.Available > 0)
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string msg = Encoding.UTF8.GetString(data);
                HandleMessage(msg);
            }
        }
        catch (SocketException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception e)
        {
            Debug.LogWarning("[PiSystemBridge] PumpUdpMessages warning: " + e.Message);
        }
    }

    void HandleMessage(string msg)
    {
        if (string.IsNullOrEmpty(msg)) return;

        msg = msg.Trim();
        if (string.IsNullOrEmpty(msg)) return;

        Debug.Log("[Pi] " + msg);

        if (msg.StartsWith("SLIDER1:"))
        {
            int raw;
            if (int.TryParse(msg.Substring("SLIDER1:".Length).Trim(), out raw))
                ExternalSetSlider1Raw(raw);
            return;
        }

        if (msg.StartsWith("SLIDER2:"))
        {
            int raw;
            if (int.TryParse(msg.Substring("SLIDER2:".Length).Trim(), out raw))
                ExternalSetSlider2Raw(raw);
            return;
        }

        if (msg.StartsWith("RIVER:"))
        {
            string riverName = msg.Substring("RIVER:".Length).Trim().ToLower();
            ExternalSelectRiver(riverName);
            return;
        }

        if (msg.Contains("JOYBTN"))
        {
            ExternalConfirm();
            return;
        }

        if (msg.Contains("BTN3"))
        {
            ToggleEnc3Mode();
            return;
        }

        if (msg.Contains("JOY:LEFT"))
        {
            ExternalMoveSelection(-1, true);
            return;
        }

        if (msg.Contains("JOY:RIGHT"))
        {
            ExternalMoveSelection(+1, true);
            return;
        }

        if (msg.Contains("JOY:UP"))
        {
            ExternalMoveSelection(-1, false);
            return;
        }

        if (msg.Contains("JOY:DOWN"))
        {
            ExternalMoveSelection(+1, false);
            return;
        }

        if (msg.Contains("ENC1:1") || msg.Contains("ENC1:+1"))
        {
            ExternalStepMap(+1);
            return;
        }

        if (msg.Contains("ENC1:-1"))
        {
            ExternalStepMap(-1);
            return;
        }

        if (msg.Contains("ENC2:1") || msg.Contains("ENC2:+1"))
        {
            ExternalStepMenu(+1);
            return;
        }

        if (msg.Contains("ENC2:-1"))
        {
            ExternalStepMenu(-1);
            return;
        }

        if (msg.Contains("ENC3:1") || msg.Contains("ENC3:+1"))
        {
            ExternalStepZoom(+1);
            return;
        }

        if (msg.Contains("ENC3:-1"))
        {
            ExternalStepZoom(-1);
            return;
        }

        Debug.Log("[Pi] unhandled message: " + msg);
    }

    public void ExternalStepMap(int step)
    {
        StepMap(step);
    }

    public void ExternalStepMenu(int step)
    {
        if (currentMode != ControlMode.MenuSelect)
        {
            Debug.Log("[Menu] ignored, mode=" + currentMode);
            return;
        }

        StepMenu(step);
    }

    public void ExternalMoveSelection(int step, bool isHorizontal)
    {
        HandleDirectionalInput(step, isHorizontal);
    }

    public void ExternalConfirm()
    {
        HandleJoystickClick();
    }

    public void ExternalStepZoom(int step)
    {
        if (Time.time - lastZoomTime < zoomCooldown) return;

        lastZoomTime = Time.time;
        if (enc3ZoomMode) StepZoom(step);
        else StepRotate(step);
    }

    public void ExternalSelectRiver(string riverName)
    {
        HoverRiverByName(riverName);
    }

    public void ExternalSetSlider1Raw(int raw)
    {
        currentSlider1Raw = raw;
        UpdateHorizontalSlider(raw);
        UpdateIntersectionAndRiver();
    }

    public void ExternalSetSlider2Raw(int raw)
    {
        currentSlider2Raw = raw;
        UpdateVerticalSlider(raw);
        UpdateIntersectionAndRiver();
    }

    // =========================
    // Map / River
    // =========================

    void StepMap(int step)
    {
        currentMapIndex = Wrap(currentMapIndex + step, 0, 2);
        ApplyMapIndex();
    }

    void ApplyMapIndex()
    {
        if (mapSwitcher == null) return;

        if (currentMapIndex == 0) mapSwitcher.ShowDefault();
        else if (currentMapIndex == 1) mapSwitcher.ShowSatellite();
        else if (currentMapIndex == 2) mapSwitcher.ShowTerrain();

        mapStyleUiIndex = Mathf.Clamp(currentMapIndex, 0, 2);
        SyncMapStyleSelectionFromIndex();

        Debug.Log("[Map] " + currentMapIndex);
    }

    void StepZoom(int step)
    {
        if (mapRoot == null) return;

        float currentScale = mapRoot.localScale.x;

        float factor = (step > 0) ? 1.05f : 0.95f;

        float nextScale = Mathf.Clamp(currentScale * factor, minScale, maxScale);

        mapRoot.localScale = new Vector3(nextScale, nextScale, nextScale);

        Debug.Log("[Zoom] scale = " + nextScale.ToString("F3"));
    }

    void StepRotate(int step)
    {
        if (mapRoot == null) return;

        float delta = (step > 0 ? -1f : 1f) * rotateStepDegrees;
        mapRoot.Rotate(0f, delta, 0f, Space.World);
        Debug.Log("[Rotate] y = " + mapRoot.eulerAngles.y.ToString("F1"));
    }

    void ToggleEnc3Mode()
    {
        enc3ZoomMode = !enc3ZoomMode;
        Debug.Log("[ENC3 Mode] " + (enc3ZoomMode ? "Zoom" : "Rotate"));
    }

    void UpdateHorizontalSlider(int raw)
    {
        if (sliderXContainer == null) return;

        float t = Mathf.InverseLerp(0f, 1023f, raw);
        Vector3 p = sliderXContainer.localPosition;
        p.x = Mathf.Lerp(sliderXMin, sliderXMax, t);
        sliderXContainer.localPosition = p;
    }

    void UpdateVerticalSlider(int raw)
    {
        if (sliderYContainer == null) return;

        float t = Mathf.InverseLerp(0f, 1023f, raw);
        Vector3 p = sliderYContainer.localPosition;
        p.y = Mathf.Lerp(sliderYMin, sliderYMax, t);
        sliderYContainer.localPosition = p;
    }

    void UpdateIntersectionAndRiver()
    {
        if (sliderXContainer == null || sliderYContainer == null) return;

        Vector3 intersection = new Vector3(
            sliderXContainer.position.x,
            sliderYContainer.position.y,
            sliderXContainer.position.z
        );

        if (intersectionMarker != null)
            intersectionMarker.position = intersection;

        if (Time.time - lastRiverSelectTime < riverSelectionCooldown) return;

        Collider[] hits = Physics.OverlapSphere(intersection, snapRadius);
        if (hits == null || hits.Length == 0)
            hits = Physics.OverlapSphere(intersection, snapRadius * 1.5f);
        Collider bestHit = null;
        float bestScore = float.MaxValue;
        float bestDist3D = float.MaxValue;
        Vector2 intersection2D = new Vector2(intersection.x, intersection.y);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider hit = hits[i];

            bool isRiverLabelTag = hit.CompareTag("RiverLabel");
            bool isRiverLabelLayer = hit.gameObject.layer == LayerMask.NameToLayer("RiverLabel");

            if (!isRiverLabelTag && !isRiverLabelLayer) continue;

            Vector3 closestPoint3D = hit.ClosestPoint(intersection);
            Vector2 closestPoint2D = new Vector2(closestPoint3D.x, closestPoint3D.y);
            float distance2D = Vector2.Distance(intersection2D, closestPoint2D);

            Bounds bounds = hit.bounds;
            bool insideProjectedBounds = intersection.x >= bounds.min.x && intersection.x <= bounds.max.x &&
                                       intersection.y >= bounds.min.y && intersection.y <= bounds.max.y;

            float score = insideProjectedBounds ? distance2D * 0.25f : distance2D;
            float dist3D = Vector3.Distance(intersection, closestPoint3D);

            if (score < bestScore - 0.0001f || (Mathf.Abs(score - bestScore) <= 0.0001f && dist3D < bestDist3D))
            {
                bestScore = score;
                bestDist3D = dist3D;
                bestHit = hit;
            }
        }

        if (bestHit == null) return;

        string riverName = LabelNameToRiverName(bestHit.name);
        if (string.IsNullOrEmpty(riverName)) return;

        HoverRiverByName(riverName);
    }

    void HoverRiverByName(string riverName)
    {
        if (string.IsNullOrEmpty(riverName)) return;

        if (hoverRiver == riverName && Time.time - lastRiverSelectTime < riverSelectionCooldown)
            return;

        hoverRiver = riverName;
        currentRiver = hoverRiver;
        lastRiverSelectTime = Time.time;

        HighlightRiverByName(hoverRiver);
        SyncRiverMenuSelectionFromName(hoverRiver);
        Debug.Log("[River Hover] " + hoverRiver);
    }

    void ConfirmHoveredRiver()
    {
        ConfirmRiverByName(hoverRiver);
    }

    void ConfirmRiverByName(string riverName)
    {
        if (string.IsNullOrEmpty(riverName))
        {
            Debug.Log("[River Confirm] no hover river");
            return;
        }

        bool enteringMenu = currentMode != ControlMode.MenuSelect;
        RiverSelectionSnapshot snapshot = CaptureRiverSelectionSnapshot();

        hoverRiver = riverName;
        selectedRiver = hoverRiver;
        currentRiver = selectedRiver;

        if (waterSystemManager != null)
            waterSystemManager.SelectRiver(selectedRiver);

        if (!enteringMenu)
            RestoreRiverSelectionSnapshot(snapshot);

        currentMode = ControlMode.MenuSelect;
        currentMenuIndex = enteringMenu ? MenuRi : NormalizeMenuIndex(currentMenuIndex);

        RefreshMenuReferences();
        CloseDropdown();
        ApplyMenuPanel();
        SyncLocalStateFromUI();
        EnsureSelectedInCurrentPanel(forceResetToFirst: false);

        Debug.Log("[River Confirmed] " + selectedRiver);
        Debug.Log("[Mode] Enter MenuSelect");
    }

    string LabelNameToRiverName(string labelName)
    {
        string n = labelName.ToLower();

        if (n.Contains("langat")) return "langat";
        if (n.Contains("lui")) return "lui";
        if (n.Contains("beranang")) return "beranang";
        if (n.Contains("rinching")) return "rinching";
        if (n.Contains("nilai")) return "btgnilai";
        if (n.Contains("semenyih")) return "semenyih";
        if (n.Contains("labu")) return "labu";
        if (n.Contains("jijan")) return "jijan";

        return "";
    }

    void HighlightRiverByName(string riverName)
    {
        TryResetAllRivers();

        string n = riverName.ToLower();
        RiverHighlighter target = null;

        if (n == "langat") target = sgLangat;
        else if (n == "lui") target = sgLui;
        else if (n == "beranang") target = sgBeranang;
        else if (n == "rinching") target = sgRinching;
        else if (n == "btgnilai") target = sgBtgNilai;
        else if (n == "semenyih") target = sgSemenyih;
        else if (n == "labu") target = sgLabu;
        else if (n == "jijan") target = sgJijan;

        if (target != null)
            target.SelectThisRiver();
    }

    void TryResetAllRivers()
    {
        TryResetRiver(sgLangat);
        TryResetRiver(sgLui);
        TryResetRiver(sgBeranang);
        TryResetRiver(sgRinching);
        TryResetRiver(sgBtgNilai);
        TryResetRiver(sgSemenyih);
        TryResetRiver(sgLabu);
        TryResetRiver(sgJijan);
    }

    void TryResetRiver(RiverHighlighter h)
    {
        if (h == null) return;
        h.gameObject.SendMessage("ResetRiver", SendMessageOptions.DontRequireReceiver);
    }

    // =========================
    // Menu Panel
    // =========================

    void StepMenu(int step)
    {
        int oldIndex = currentMenuIndex;
        currentMenuIndex = GetSteppedMenuIndex(currentMenuIndex, step);

        CloseDropdown();
        ApplyMenuPanel();
        EnsureSelectedInCurrentPanel(forceResetToFirst: false);

        Debug.Log("[Menu] " + oldIndex + " -> " + currentMenuIndex + " : " + menuNames[NormalizeMenuIndex(currentMenuIndex)]);
    }

    void ApplyMenuPanel()
    {
        bool inMenu = currentMode == ControlMode.MenuSelect;

        if (classPanel != null) classPanel.SetActive(inMenu);
        if (dataPanel != null) dataPanel.SetActive(inMenu);
        if (datePanel != null) datePanel.SetActive(false);
        if (parameterPanel != null) parameterPanel.SetActive(false);

        RefreshMenuReferences();
        UpdateMenuFocusVisuals();
    }

    GameObject GetCurrentPanelRoot()
    {
        switch (NormalizeMenuIndex(currentMenuIndex))
        {
            case MenuRi: return classPanel;
            case MenuData: return dataPanel;
        }
        return null;
    }

    bool HasDedicatedDatePanel()
    {
        return datePanel != null && datePanel != dataPanel;
    }

    int NormalizeMenuIndex(int idx)
    {
        return Mathf.Clamp(idx, 0, MenuCount - 1);
    }

    int GetSteppedMenuIndex(int current, int step)
    {
        return Wrap(current + step, 0, MenuCount - 1);
    }

    bool UseMergedDataFlow()
    {
        return waterSystemManager != null;
    }

    bool IsMergedDataMenuActive()
    {
        return currentMode == ControlMode.MenuSelect && NormalizeMenuIndex(currentMenuIndex) == MenuData && UseMergedDataFlow();
    }

    // =========================
    // Joystick / UI Navigation
    // =========================

    void HandleJoystickClick()
    {
        Debug.Log("[JOYBTN] mode=" + currentMode + ", menu=" + currentMenuIndex);

        if (currentMode == ControlMode.MapSelect)
        {
            ConfirmHoveredRiver();
            return;
        }

        if (currentMode != ControlMode.MenuSelect)
            return;

        if (dropdownOpen && dropdownOpenAndPickMode)
        {
            CloseDropdown();
            ForceRefreshLineChart();
            SyncLocalStateFromUI();
            return;
        }

        if (IsMergedDataMenuActive())
        {
            if (!TryActivateMergedDataCurrent())
                ConfirmCurrentMenuSelection();

            ForceRefreshLineChart();
            SyncLocalStateFromUI();
            return;
        }

        if (!TryClickCurrentSelectable())
        {
            // 没有 panel / 没有 selectable 时，退回旧业务逻辑
            ConfirmCurrentMenuSelection();
        }

        ForceRefreshLineChart();
        SyncLocalStateFromUI();
    }

    void HandleDirectionalInput(int step, bool isHorizontal)
    {
        Debug.Log("[JOY] step=" + step + ", horizontal=" + isHorizontal + ", mode=" + currentMode + ", menu=" + currentMenuIndex);

        if (Time.time - lastJoyMoveTime < joyMoveCooldown)
            return;

        lastJoyMoveTime = Time.time;

        if (currentMode != ControlMode.MenuSelect)
            return;

        if (dropdownOpen && dropdownOpenAndPickMode)
        {
            if (!isHorizontal)
            {
                if (Time.time >= nextDropdownStepTime)
                {
                    nextDropdownStepTime = Time.time + dropdownStepCooldown;
                    DropdownStep(step);
                }
                return;
            }

            CloseDropdown();
            ForceRefreshLineChart();
            SyncLocalStateFromUI();
        }

        if (IsMergedDataMenuActive())
        {
            if (MoveInMergedDataGrid(step, isHorizontal))
                return;
        }

        // 优先使用真实 UI 导航
        if (MoveSelectionInCurrentPanel(step, isHorizontal))
            return;

        // 如果 panel 没有可导航 UI，再退回旧逻辑
        switch (NormalizeMenuIndex(currentMenuIndex))
        {
            case MenuRi:
                StepClass(step);
                break;
            case MenuData:
                StepData(step);
                break;
            case MenuMapStyle:
                StepMapStyleSelection(step);
                break;
            case MenuRiver:
                break;
        }
    }

    bool MoveSelectionInCurrentPanel(int step, bool isHorizontal)
    {
        if (EventSystem.current == null) return false;

        List<Selectable> list = GetCurrentNavigationSelectables();
        if (list.Count == 0) return false;

        RefInt idxRef = GetPanelIndexRef();
        if (idxRef == null) return false;

        int current = Mathf.Clamp(idxRef.Value, 0, list.Count - 1);

        // 先确保当前有选中
        if (EventSystem.current.currentSelectedGameObject == null || forceSelectionOutOfList(list))
        {
            idxRef.Value = current;
            EventSystem.current.SetSelectedGameObject(list[current].gameObject);
            return true;
        }

        // 上下左右先统一按线性移动
        int next = Mathf.Clamp(current + step, 0, list.Count - 1);
        if (next != current)
        {
            idxRef.Value = next;
            EventSystem.current.SetSelectedGameObject(list[next].gameObject);
        }

        return true;
    }

    bool forceSelectionOutOfList(List<Selectable> list)
    {
        GameObject go = EventSystem.current.currentSelectedGameObject;
        if (go == null) return true;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && list[i].gameObject == go)
                return false;
        }
        return true;
    }

    void EnsureSelectedInCurrentPanel(bool forceResetToFirst)
    {
        if (EventSystem.current == null) return;

        if (IsMergedDataMenuActive())
        {
            EnsureDateUiReady();

            if (forceResetToFirst || !IsMergedDataSlotValid(dataUiIndex))
                dataUiIndex = GetFirstMergedDataSlot();

            SetMergedDataSelection(dataUiIndex);
            return;
        }

        List<Selectable> list = GetCurrentNavigationSelectables();
        if (list.Count == 0) return;

        RefInt idxRef = GetPanelIndexRef();
        if (idxRef == null) return;

        if (forceResetToFirst)
            idxRef.Value = 0;

        int idx = Mathf.Clamp(idxRef.Value, 0, list.Count - 1);
        EventSystem.current.SetSelectedGameObject(list[idx].gameObject);
    }

    bool TryClickCurrentSelectable()
    {
        if (EventSystem.current == null) return false;

        List<Selectable> list = GetCurrentNavigationSelectables();
        RefInt idxRef = GetPanelIndexRef();
        Selectable current = null;

        if (list.Count > 0 && idxRef != null)
        {
            int idx = Mathf.Clamp(idxRef.Value, 0, list.Count - 1);
            current = list[idx];
        }

        if (NormalizeMenuIndex(currentMenuIndex) == MenuRiver)
        {
            if (!string.IsNullOrEmpty(hoverRiver) && !string.Equals(hoverRiver, selectedRiver, StringComparison.OrdinalIgnoreCase))
            {
                ConfirmHoveredRiver();
                return true;
            }

            string selectableRiver;
            if (TryGetRiverNameFromSelectable(current, out selectableRiver))
            {
                ConfirmRiverByName(selectableRiver);
                return true;
            }
        }

        if (list.Count == 0) return false;
        if (idxRef == null) return false;
        if (current == null) return false;

        EventSystem.current.SetSelectedGameObject(current.gameObject);

        if (dropdownOpenAndPickMode && TryOpenDropdown(current))
            return true;

        ClickSelectable(current.gameObject);
        if (NormalizeMenuIndex(currentMenuIndex) == MenuMapStyle)
            SyncMapStyleStateFromSelectable(current);
        return true;
    }

    bool TryOpenDropdown(Selectable current)
    {
        PrepareDropdownBeforeOpen(current);

        TMP_Dropdown tmp = current.GetComponent<TMP_Dropdown>();
        if (tmp != null)
        {
            if (tmp.options == null || tmp.options.Count == 0) return false;
            tmp.Show();
            dropdownOpen = true;
            openTmpDropdown = tmp;
            openLegacyDropdown = null;
            return true;
        }

        Dropdown legacy = current.GetComponent<Dropdown>();
        if (legacy != null)
        {
            if (legacy.options == null || legacy.options.Count == 0) return false;
            legacy.Show();
            dropdownOpen = true;
            openLegacyDropdown = legacy;
            openTmpDropdown = null;
            return true;
        }

        return false;
    }

    void PrepareDropdownBeforeOpen(Selectable current)
    {
        if (current == null) return;
        EnsureDateUiReady();
    }

    void DropdownStep(int step)
    {
        if (openTmpDropdown != null)
        {
            int n = openTmpDropdown.options.Count;
            if (n <= 0) return;
            int v = Mathf.Clamp(openTmpDropdown.value + step, 0, n - 1);
            if (v != openTmpDropdown.value)
            {
                openTmpDropdown.value = v;
                openTmpDropdown.RefreshShownValue();

                if (waterSystemManager != null && openTmpDropdown == waterSystemManager.startYearDropdown)
                    waterSystemManager.UpdateDurationDropdownOptions();
            }
            return;
        }

        if (openLegacyDropdown != null)
        {
            int n = openLegacyDropdown.options.Count;
            if (n <= 0) return;
            int v = Mathf.Clamp(openLegacyDropdown.value + step, 0, n - 1);
            if (v != openLegacyDropdown.value)
            {
                openLegacyDropdown.value = v;
                openLegacyDropdown.RefreshShownValue();
            }
        }
    }

    void CloseDropdown()
    {
        if (openTmpDropdown != null) openTmpDropdown.Hide();
        if (openLegacyDropdown != null) openLegacyDropdown.Hide();

        dropdownOpen = false;
        openTmpDropdown = null;
        openLegacyDropdown = null;
    }

    void ClickSelectable(GameObject go)
    {
        if (go == null || EventSystem.current == null) return;

        EventSystem.current.SetSelectedGameObject(go);

        PointerEventData ped = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(go, ped, ExecuteEvents.pointerClickHandler);
        ExecuteEvents.Execute(go, ped, ExecuteEvents.submitHandler);
    }

    Selectable GetMergedDataSelectable(int slot)
    {
        if (waterSystemManager == null) return null;

        switch (slot)
        {
            case 0:
                return GetToggleFromArray(waterSystemManager.dataToggles, 0);
            case 1:
                return GetToggleFromArray(waterSystemManager.dataToggles, 1);
            case 2:
                return waterSystemManager.startMonthDropdown;
            case 3:
                return waterSystemManager.startYearDropdown;
            case 4:
                return waterSystemManager.durationMonthToggle;
            case 5:
                return waterSystemManager.durationYearToggle;
            case 6:
                return waterSystemManager.durationValueDropdown;
            case 7:
                return GetToggleFromArray(waterSystemManager.mainCategoryToggles, 0);
            case 8:
                return GetToggleFromArray(waterSystemManager.mainCategoryToggles, 1);
            case 9:
                return GetToggleFromArray(waterSystemManager.mainCategoryToggles, 2);
        }

        return null;
    }

    Toggle GetToggleFromArray(Toggle[] toggles, int index)
    {
        if (toggles == null || index < 0 || index >= toggles.Length) return null;
        return toggles[index];
    }

    bool IsMergedDataSlotValid(int slot)
    {
        Selectable s = GetMergedDataSelectable(slot);
        return s != null && s.IsInteractable() && s.gameObject.activeInHierarchy;
    }

    int GetFirstMergedDataSlot()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (IsMergedDataSlotValid(i)) return i;
        }
        return 0;
    }

    void SetMergedDataSelection(int slot)
    {
        if (!IsMergedDataSlotValid(slot) || EventSystem.current == null) return;

        dataUiIndex = Mathf.Clamp(slot, 0, 9);
        if (slot == 0 || slot == 2 || slot == 4) mergedDataRow = 0;
        else if (slot == 1 || slot == 3 || slot == 5) mergedDataRow = 1;
        else if (slot >= 7 && slot <= 9) parameterUiIndex = slot - 7;

        EventSystem.current.SetSelectedGameObject(GetMergedDataSelectable(dataUiIndex).gameObject);
    }

    bool MoveInMergedDataGrid(int step, bool isHorizontal)
    {
        EnsureDateUiReady();

        int current = IsMergedDataSlotValid(dataUiIndex) ? dataUiIndex : GetFirstMergedDataSlot();
        int next = GetNextMergedDataSlot(current, step, isHorizontal);

        if (!IsMergedDataSlotValid(next))
            return false;

        SetMergedDataSelection(next);
        return true;
    }

    int GetNextMergedDataSlot(int current, int step, bool isHorizontal)
    {
        int direction = step < 0 ? -1 : 1;

        if (isHorizontal)
        {
            switch (current)
            {
                case 0: return direction > 0 ? 2 : 0;
                case 1: return direction > 0 ? 3 : 1;
                case 2: return direction > 0 ? 4 : 0;
                case 3: return direction > 0 ? 5 : 1;
                case 4: return direction > 0 ? 6 : 2;
                case 5: return direction > 0 ? 6 : 3;
                case 6: return direction < 0 ? (mergedDataRow == 0 ? 4 : 5) : 6;
                case 7: return direction > 0 ? 8 : 7;
                case 8: return direction > 0 ? 9 : 7;
                case 9: return direction < 0 ? 8 : 9;
            }
        }

        switch (current)
        {
            case 0: return direction > 0 ? 1 : 0;
            case 1: return direction < 0 ? 0 : 7;
            case 2: return direction > 0 ? 3 : 2;
            case 3: return direction < 0 ? 2 : 8;
            case 4: return direction > 0 ? 5 : 4;
            case 5: return direction < 0 ? 4 : 9;
            case 6: return direction < 0 ? 4 : 5;
            case 7: return direction < 0 ? 1 : 7;
            case 8: return direction < 0 ? 3 : 8;
            case 9: return direction < 0 ? 5 : 9;
        }

        return current;
    }

    bool TryActivateMergedDataCurrent()
    {
        EnsureDateUiReady();

        int slot = IsMergedDataSlotValid(dataUiIndex) ? dataUiIndex : GetFirstMergedDataSlot();
        Selectable current = GetMergedDataSelectable(slot);
        if (current == null) return false;

        SetMergedDataSelection(slot);

        switch (slot)
        {
            case 0:
                ApplyDataToggleDirect(0);
                return true;
            case 1:
                ApplyDataToggleDirect(1);
                return true;
            case 2:
            case 3:
            case 6:
                return TryOpenDropdown(current);
            case 4:
                SetDurationTypeDirect(true);
                return true;
            case 5:
                SetDurationTypeDirect(false);
                return true;
            case 7:
            case 8:
            case 9:
                parameterUiIndex = slot - 7;
                ApplyParameter();
                return true;
        }

        return false;
    }

    void ApplyDataToggleDirect(int index)
    {
        if (waterSystemManager == null || waterSystemManager.dataToggles == null) return;
        if (index < 0 || index >= waterSystemManager.dataToggles.Length) return;
        if (waterSystemManager.dataToggles[index] == null) return;

        waterSystemManager.dataToggles[index].SetIsOnWithoutNotify(true);
        waterSystemManager.OnDataToggleChanged(index);
    }

    void SetDurationTypeDirect(bool useMonth)
    {
        if (waterSystemManager == null) return;

        if (waterSystemManager.durationMonthToggle != null)
            waterSystemManager.durationMonthToggle.SetIsOnWithoutNotify(useMonth);

        if (waterSystemManager.durationYearToggle != null)
            waterSystemManager.durationYearToggle.SetIsOnWithoutNotify(!useMonth);

        waterSystemManager.UpdateDurationDropdownOptions();
    }

    void EnsureDateUiReady()
    {
        if (waterSystemManager == null) return;

        EnsureMonthDropdownOptions();

        if (waterSystemManager.startYearDropdown != null &&
            (waterSystemManager.startYearDropdown.options == null || waterSystemManager.startYearDropdown.options.Count == 0))
        {
            waterSystemManager.RefreshYearOptions();
        }

        bool monthOn = waterSystemManager.durationMonthToggle != null && waterSystemManager.durationMonthToggle.isOn;
        bool yearOn = waterSystemManager.durationYearToggle != null && waterSystemManager.durationYearToggle.isOn;

        if (!monthOn && !yearOn)
        {
            if (waterSystemManager.durationMonthToggle != null)
                waterSystemManager.durationMonthToggle.SetIsOnWithoutNotify(true);

            if (waterSystemManager.durationYearToggle != null)
                waterSystemManager.durationYearToggle.SetIsOnWithoutNotify(false);
        }

        if (waterSystemManager.durationValueDropdown != null &&
            (waterSystemManager.durationValueDropdown.options == null || waterSystemManager.durationValueDropdown.options.Count == 0))
        {
            waterSystemManager.UpdateDurationDropdownOptions();
        }
    }

    void EnsureMonthDropdownOptions()
    {
        if (waterSystemManager == null || waterSystemManager.startMonthDropdown == null) return;

        TMP_Dropdown dropdown = waterSystemManager.startMonthDropdown;
        if (dropdown.options != null && dropdown.options.Count > 0) return;

        List<string> months = new List<string>();
        for (int i = 1; i <= 12; i++)
            months.Add(i.ToString("00"));

        dropdown.ClearOptions();
        dropdown.AddOptions(months);
        dropdown.value = Mathf.Clamp(dropdown.value, 0, months.Count - 1);
        dropdown.RefreshShownValue();
    }

    List<Selectable> CollectSelectables(GameObject root)
    {
        List<Selectable> list = new List<Selectable>();
        if (root == null) return list;

        Selectable[] arr = root.GetComponentsInChildren<Selectable>(true);

        foreach (Selectable s in arr)
        {
            if (s == null) continue;
            if (!s.IsInteractable()) continue;
            if (!s.gameObject.activeInHierarchy) continue;
            list.Add(s);
        }

        SortSelectableList(list);

        return list;
    }

    List<Selectable> GetCurrentNavigationSelectables()
    {
        switch (NormalizeMenuIndex(currentMenuIndex))
        {
            case MenuRi:
                return CollectSelectables(classPanel);
            case MenuRiver:
                RefreshMenuReferences();
                return new List<Selectable>(riverMenuSelectables);
            case MenuMapStyle:
                RefreshMenuReferences();
                return new List<Selectable>(mapStyleSelectables);
        }

        return new List<Selectable>();
    }

    void SortSelectableList(List<Selectable> list)
    {
        if (list == null) return;

        list.Sort((a, b) =>
        {
            int ay = Mathf.RoundToInt(-a.transform.position.y * 1000f);
            int by = Mathf.RoundToInt(-b.transform.position.y * 1000f);
            if (ay != by) return ay.CompareTo(by);

            int ax = Mathf.RoundToInt(a.transform.position.x * 1000f);
            int bx = Mathf.RoundToInt(b.transform.position.x * 1000f);
            return ax.CompareTo(bx);
        });
    }

    void RefreshMenuReferences()
    {
        mapStyleSelectables.Clear();
        riverMenuSelectables.Clear();

        Selectable[] arr = Resources.FindObjectsOfTypeAll<Selectable>();
        for (int i = 0; i < arr.Length; i++)
        {
            Selectable s = arr[i];
            if (s == null) continue;
            if (!s.gameObject.scene.IsValid()) continue;

            if (IsMapStyleSelectable(s)) mapStyleSelectables.Add(s);
            else if (IsRiverMenuSelectable(s)) riverMenuSelectables.Add(s);
        }

        SortSelectableList(mapStyleSelectables);
        SortSelectableList(riverMenuSelectables);
    }

    bool IsMapStyleSelectable(Selectable selectable)
    {
        return HasPersistentMethod(selectable, typeof(MapSwitcher), "ShowDefault") ||
               HasPersistentMethod(selectable, typeof(MapSwitcher), "ShowSatellite") ||
               HasPersistentMethod(selectable, typeof(MapSwitcher), "ShowTerrain");
    }

    bool IsRiverMenuSelectable(Selectable selectable)
    {
        return HasPersistentMethod(selectable, typeof(WaterSystemManager), "SelectRiver");
    }

    bool HasPersistentMethod(Selectable selectable, Type targetType, string methodName)
    {
        if (selectable == null) return false;

        Button button = selectable.GetComponent<Button>();
        if (button != null && EventHasPersistentMethod(button.onClick, targetType, methodName))
            return true;

        Toggle toggle = selectable.GetComponent<Toggle>();
        if (toggle != null && EventHasPersistentMethod(toggle.onValueChanged, targetType, methodName))
            return true;

        return false;
    }

    bool EventHasPersistentMethod(UnityEventBase evt, Type targetType, string methodName)
    {
        if (evt == null) return false;

        int count = evt.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            UnityEngine.Object target = evt.GetPersistentTarget(i);
            if (target == null) continue;
            if (!targetType.IsAssignableFrom(target.GetType())) continue;
            if (string.Equals(evt.GetPersistentMethodName(i), methodName, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    RiverSelectionSnapshot CaptureRiverSelectionSnapshot()
    {
        RiverSelectionSnapshot snapshot = new RiverSelectionSnapshot();
        snapshot.classIndex = classUiIndex;
        snapshot.dataToggleIndex = waterSystemManager != null ? GetSelectedToggleIndex(waterSystemManager.dataToggles, 0) : 0;
        snapshot.dataSlotIndex = dataUiIndex;
        snapshot.parameterIndex = parameterUiIndex;
        snapshot.startMonth = waterSystemManager != null ? SafeDropdownText(waterSystemManager.startMonthDropdown) : "";
        snapshot.startYear = waterSystemManager != null ? SafeDropdownText(waterSystemManager.startYearDropdown) : "";
        snapshot.durationValue = waterSystemManager != null ? SafeDropdownText(waterSystemManager.durationValueDropdown) : "";
        snapshot.useMonthDuration = waterSystemManager != null && waterSystemManager.durationMonthToggle != null && waterSystemManager.durationMonthToggle.isOn;
        snapshot.hadParameterSelection = AnyToggleOn(waterSystemManager != null ? waterSystemManager.mainCategoryToggles : null);
        return snapshot;
    }

    void RestoreRiverSelectionSnapshot(RiverSelectionSnapshot snapshot)
    {
        if (waterSystemManager == null) return;

        classUiIndex = Mathf.Clamp(snapshot.classIndex, 0, 4);
        dataUiIndex = Mathf.Clamp(snapshot.dataSlotIndex, 0, 9);
        parameterUiIndex = Mathf.Clamp(snapshot.parameterIndex, 0, 2);

        ApplyClass();
        ApplyDataToggleDirect(Mathf.Clamp(snapshot.dataToggleIndex, 0, 1));
        EnsureDateUiReady();
        SetDurationTypeDirect(snapshot.useMonthDuration);
        SetDropdownValueByText(waterSystemManager.startMonthDropdown, snapshot.startMonth);
        SetDropdownValueByText(waterSystemManager.startYearDropdown, snapshot.startYear);
        waterSystemManager.UpdateDurationDropdownOptions();
        SetDropdownValueByText(waterSystemManager.durationValueDropdown, snapshot.durationValue);

        if (snapshot.hadParameterSelection)
        {
            ApplyParameter();
        }
        else if (waterSystemManager.paramLineChart != null)
        {
            waterSystemManager.paramLineChart.gameObject.SetActive(false);
        }
    }

    bool AnyToggleOn(Toggle[] toggles)
    {
        if (toggles == null) return false;
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i] != null && toggles[i].isOn) return true;
        }
        return false;
    }

    void SetDropdownValueByText(TMP_Dropdown dropdown, string text)
    {
        if (dropdown == null || dropdown.options == null || dropdown.options.Count == 0 || string.IsNullOrEmpty(text)) return;

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (string.Equals(dropdown.options[i].text, text, StringComparison.OrdinalIgnoreCase))
            {
                dropdown.value = i;
                dropdown.RefreshShownValue();
                return;
            }
        }
    }

    void SyncRiverMenuSelectionFromName(string riverName)
    {
        if (string.IsNullOrEmpty(riverName)) return;

        RefreshMenuReferences();
        int idx = FindSelectableIndexForRiver(riverMenuSelectables, riverName);
        if (idx < 0) return;

        riverUiIndex = idx;
        if (currentMode == ControlMode.MenuSelect && NormalizeMenuIndex(currentMenuIndex) == MenuRiver && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(riverMenuSelectables[idx].gameObject);
    }

    int FindSelectableIndexForRiver(List<Selectable> list, string riverName)
    {
        if (list == null || list.Count == 0 || string.IsNullOrEmpty(riverName)) return -1;

        string target = NormalizeRiverNameForMatch(riverName);
        for (int i = 0; i < list.Count; i++)
        {
            if (SelectableMatchesRiverName(list[i], target))
                return i;
        }

        return -1;
    }

    bool SelectableMatchesRiverName(Selectable selectable, string normalizedRiver)
    {
        if (selectable == null || string.IsNullOrEmpty(normalizedRiver)) return false;

        string ownName = NormalizeRiverNameForMatch(selectable.gameObject.name);
        if (ownName.Contains(normalizedRiver) || normalizedRiver.Contains(ownName))
            return true;

        TMP_Text[] tmpTexts = selectable.GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < tmpTexts.Length; i++)
        {
            string textName = NormalizeRiverNameForMatch(tmpTexts[i].text);
            if (textName.Contains(normalizedRiver) || normalizedRiver.Contains(textName))
                return true;
        }

        Text[] texts = selectable.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            string textName = NormalizeRiverNameForMatch(texts[i].text);
            if (textName.Contains(normalizedRiver) || normalizedRiver.Contains(textName))
                return true;
        }

        return false;
    }

    bool TryGetRiverNameFromSelectable(Selectable selectable, out string riverName)
    {
        riverName = "";
        if (selectable == null) return false;

        riverName = ResolveRiverName(selectable.gameObject.name);
        if (!string.IsNullOrEmpty(riverName)) return true;

        TMP_Text[] tmpTexts = selectable.GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < tmpTexts.Length; i++)
        {
            riverName = ResolveRiverName(tmpTexts[i].text);
            if (!string.IsNullOrEmpty(riverName)) return true;
        }

        Text[] texts = selectable.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            riverName = ResolveRiverName(texts[i].text);
            if (!string.IsNullOrEmpty(riverName)) return true;
        }

        riverName = "";
        return false;
    }

    string ResolveRiverName(string value)
    {
        string normalized = NormalizeRiverNameForMatch(value);
        if (string.IsNullOrEmpty(normalized)) return "";

        if (normalized.Contains("langat")) return "langat";
        if (normalized.Contains("lui")) return "lui";
        if (normalized.Contains("beranang")) return "beranang";
        if (normalized.Contains("rinching")) return "rinching";
        if (normalized.Contains("btgnilai") || normalized.Contains("nilai")) return "btgnilai";
        if (normalized.Contains("semenyih")) return "semenyih";
        if (normalized.Contains("labu")) return "labu";
        if (normalized.Contains("jijan")) return "jijan";
        return "";
    }

    string NormalizeRiverNameForMatch(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";

        string s = value.ToLowerInvariant();
        s = s.Replace("sungai", "");
        s = s.Replace("sg", "");
        s = s.Replace("batang", "btg");
        s = s.Replace(" ", "");
        s = s.Replace("-", "");
        s = s.Replace("_", "");
        s = s.Replace("(", "");
        s = s.Replace(")", "");
        s = s.Replace(".", "");
        return s.Trim();
    }

    void UpdateMenuFocusVisuals()
    {
        bool inMenu = currentMode == ControlMode.MenuSelect;
        int activeIndex = NormalizeMenuIndex(currentMenuIndex);

        for (int i = 0; i < MenuCount; i++)
            UpdateMenuFocusFrame(i, inMenu && i == activeIndex);
    }

    void UpdateMenuFocusFrame(int menuIndex, bool visible)
    {
        RectTransform frame = menuFocusFrames[menuIndex];
        if (!visible)
        {
            if (frame != null) frame.gameObject.SetActive(false);
            return;
        }

        Canvas canvas;
        Vector2 center;
        Vector2 size;
        if (!TryGetMenuFocusBounds(menuIndex, out canvas, out center, out size))
        {
            if (frame != null) frame.gameObject.SetActive(false);
            return;
        }

        frame = EnsureMenuFocusFrame(menuIndex, canvas);
        frame.gameObject.SetActive(true);
        frame.SetAsLastSibling();
        frame.anchoredPosition = center;
        frame.sizeDelta = size + Vector2.one * (menuFocusPadding * 2f);
        UpdateMenuFocusFrameLayout(frame);
    }

    RectTransform EnsureMenuFocusFrame(int menuIndex, Canvas canvas)
    {
        RectTransform frame = menuFocusFrames[menuIndex];
        if (frame == null)
        {
            GameObject go = new GameObject("PiFocus_" + menuNames[menuIndex], typeof(RectTransform));
            frame = go.GetComponent<RectTransform>();
            frame.anchorMin = new Vector2(0.5f, 0.5f);
            frame.anchorMax = new Vector2(0.5f, 0.5f);
            frame.pivot = new Vector2(0.5f, 0.5f);

            menuFocusFrames[menuIndex] = frame;
        }

        if (frame.transform.parent != canvas.transform)
            frame.SetParent(canvas.transform, false);

        EnsureMenuFocusBorder(frame, "Top");
        EnsureMenuFocusBorder(frame, "Bottom");
        EnsureMenuFocusBorder(frame, "Left");
        EnsureMenuFocusBorder(frame, "Right");

        return frame;
    }

    void EnsureMenuFocusBorder(RectTransform frame, string borderName)
    {
        Transform existing = frame.Find(borderName);
        if (existing != null) return;

        GameObject go = new GameObject(borderName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform border = go.GetComponent<RectTransform>();
        border.SetParent(frame, false);

        Image image = go.GetComponent<Image>();
        image.color = menuFocusColor;
        image.raycastTarget = false;
    }

    void UpdateMenuFocusFrameLayout(RectTransform frame)
    {
        if (frame == null) return;

        float width = Mathf.Max(1f, frame.sizeDelta.x);
        float height = Mathf.Max(1f, frame.sizeDelta.y);
        float thickness = Mathf.Max(1f, menuFocusThickness);

        UpdateMenuFocusBorder(frame, "Top", new Vector2(0.5f, 1f), new Vector2(width, thickness), new Vector2(0f, -thickness * 0.5f));
        UpdateMenuFocusBorder(frame, "Bottom", new Vector2(0.5f, 0f), new Vector2(width, thickness), new Vector2(0f, thickness * 0.5f));
        UpdateMenuFocusBorder(frame, "Left", new Vector2(0f, 0.5f), new Vector2(thickness, height), new Vector2(thickness * 0.5f, 0f));
        UpdateMenuFocusBorder(frame, "Right", new Vector2(1f, 0.5f), new Vector2(thickness, height), new Vector2(-thickness * 0.5f, 0f));
    }

    void UpdateMenuFocusBorder(RectTransform frame, string borderName, Vector2 anchor, Vector2 size, Vector2 anchoredPosition)
    {
        Transform t = frame.Find(borderName);
        if (t == null) return;

        RectTransform border = t as RectTransform;
        if (border == null) return;

        border.anchorMin = anchor;
        border.anchorMax = anchor;
        border.pivot = new Vector2(0.5f, 0.5f);
        border.sizeDelta = size;
        border.anchoredPosition = anchoredPosition;

        Image image = border.GetComponent<Image>();
        if (image != null) image.color = menuFocusColor;
    }

    bool TryGetMenuFocusBounds(int menuIndex, out Canvas canvas, out Vector2 center, out Vector2 size)
    {
        canvas = null;
        center = Vector2.zero;
        size = Vector2.zero;

        if (menuIndex == MenuRi)
            return TryGetRectBounds(classPanel != null ? classPanel.transform as RectTransform : null, out canvas, out center, out size);

        if (menuIndex == MenuData)
            return TryGetRectBounds(dataPanel != null ? dataPanel.transform as RectTransform : null, out canvas, out center, out size);

        RefreshMenuReferences();
        if (menuIndex == MenuMapStyle)
            return TryGetSelectablesBounds(mapStyleSelectables, out canvas, out center, out size);

        if (menuIndex == MenuRiver)
            return TryGetSelectablesBounds(riverMenuSelectables, out canvas, out center, out size);

        return false;
    }

    bool TryGetRectBounds(RectTransform rect, out Canvas canvas, out Vector2 center, out Vector2 size)
    {
        canvas = null;
        center = Vector2.zero;
        size = Vector2.zero;
        if (rect == null || !rect.gameObject.activeInHierarchy) return false;

        canvas = rect.GetComponentInParent<Canvas>();
        if (canvas == null) return false;

        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        return TryConvertWorldCornersToCanvasRect(canvas, corners, out center, out size);
    }

    bool TryGetSelectablesBounds(List<Selectable> list, out Canvas canvas, out Vector2 center, out Vector2 size)
    {
        canvas = null;
        center = Vector2.zero;
        size = Vector2.zero;
        if (list == null || list.Count == 0) return false;

        RectTransform canvasRect = null;
        Vector2 min = Vector2.zero;
        Vector2 max = Vector2.zero;
        bool hasPoint = false;

        for (int i = 0; i < list.Count; i++)
        {
            Selectable selectable = list[i];
            if (selectable == null || !selectable.gameObject.activeInHierarchy) continue;

            RectTransform rect = selectable.transform as RectTransform;
            if (rect == null) continue;

            if (canvas == null)
            {
                canvas = rect.GetComponentInParent<Canvas>();
                if (canvas == null) continue;
                canvasRect = canvas.transform as RectTransform;
                if (canvasRect == null) return false;
            }

            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);

            for (int c = 0; c < 4; c++)
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[c]);
                Vector2 localPoint;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.worldCamera, out localPoint))
                    continue;

                if (!hasPoint)
                {
                    min = localPoint;
                    max = localPoint;
                    hasPoint = true;
                }
                else
                {
                    min = Vector2.Min(min, localPoint);
                    max = Vector2.Max(max, localPoint);
                }
            }
        }

        if (!hasPoint || canvas == null) return false;

        center = (min + max) * 0.5f;
        size = max - min;
        return true;
    }

    bool TryConvertWorldCornersToCanvasRect(Canvas canvas, Vector3[] corners, out Vector2 center, out Vector2 size)
    {
        center = Vector2.zero;
        size = Vector2.zero;
        if (canvas == null || corners == null || corners.Length < 4) return false;

        RectTransform canvasRect = canvas.transform as RectTransform;
        if (canvasRect == null) return false;

        Vector2 min = Vector2.zero;
        Vector2 max = Vector2.zero;
        bool hasPoint = false;

        for (int i = 0; i < 4; i++)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[i]);
            Vector2 localPoint;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.worldCamera, out localPoint))
                continue;

            if (!hasPoint)
            {
                min = localPoint;
                max = localPoint;
                hasPoint = true;
            }
            else
            {
                min = Vector2.Min(min, localPoint);
                max = Vector2.Max(max, localPoint);
            }
        }

        if (!hasPoint) return false;

        center = (min + max) * 0.5f;
        size = max - min;
        return true;
    }

    RefInt GetPanelIndexRef()
    {
        switch (NormalizeMenuIndex(currentMenuIndex))
        {
            case MenuRi: return new RefInt(() => classUiIndex, v => classUiIndex = v);
            case MenuData: return new RefInt(() => dataUiIndex, v => dataUiIndex = v);
            case MenuMapStyle: return new RefInt(() => mapStyleUiIndex, v => mapStyleUiIndex = v);
            case MenuRiver: return new RefInt(() => riverUiIndex, v => riverUiIndex = v);
        }
        return null;
    }

    class RefInt
    {
        private Func<int> get;
        private Action<int> set;

        public RefInt(Func<int> get, Action<int> set)
        {
            this.get = get;
            this.set = set;
        }

        public int Value
        {
            get { return get(); }
            set { set(value); }
        }
    }

    // =========================
    // Legacy business fallback
    // =========================

    void ConfirmCurrentMenuSelection()
    {
        switch (NormalizeMenuIndex(currentMenuIndex))
        {
            case MenuRi:
                ApplyClass();
                break;
            case MenuData:
                ApplyData();
                break;
            case MenuMapStyle:
                ApplyMapStyleSelection();
                break;
            case MenuRiver:
                ConfirmHoveredRiver();
                break;
        }
    }

    void StepClass(int step)
    {
        classUiIndex = Wrap(classUiIndex + step, 0, 4);
        Debug.Log("[Class Preview] " + ClassIndexToName(classUiIndex));
    }

    void ApplyClass()
    {
        if (waterSystemManager == null) return;

        if (waterSystemManager.classToggles != null &&
            classUiIndex >= 0 &&
            classUiIndex < waterSystemManager.classToggles.Length &&
            waterSystemManager.classToggles[classUiIndex] != null)
        {
            waterSystemManager.classToggles[classUiIndex].SetIsOnWithoutNotify(true);
        }

        waterSystemManager.OnClassToggleChanged(classUiIndex);
        Debug.Log("[Class Apply] " + ClassIndexToName(classUiIndex));
    }

    string ClassIndexToName(int idx)
    {
        string[] names = { "All", "I", "II", "III", "IV" };
        return names[Mathf.Clamp(idx, 0, names.Length - 1)];
    }

    void StepData(int step)
    {
        dataUiIndex = Wrap(dataUiIndex + step, 0, 1);
        Debug.Log("[Data Preview] " + (dataUiIndex == 0 ? "Existing" : "Prediction"));
    }

    void StepMapStyleSelection(int step)
    {
        mapStyleUiIndex = Wrap(mapStyleUiIndex + step, 0, 2);
        Debug.Log("[MapStyle Preview] " + mapStyleUiIndex);
    }

    void ApplyMapStyleSelection()
    {
        currentMapIndex = Mathf.Clamp(mapStyleUiIndex, 0, 2);
        ApplyMapIndex();
    }

    void SyncMapStyleStateFromSelectable(Selectable selectable)
    {
        if (HasPersistentMethod(selectable, typeof(MapSwitcher), "ShowDefault")) currentMapIndex = 0;
        else if (HasPersistentMethod(selectable, typeof(MapSwitcher), "ShowSatellite")) currentMapIndex = 1;
        else if (HasPersistentMethod(selectable, typeof(MapSwitcher), "ShowTerrain")) currentMapIndex = 2;

        mapStyleUiIndex = Mathf.Clamp(currentMapIndex, 0, 2);
        SyncMapStyleSelectionFromIndex();
    }

    void SyncMapStyleSelectionFromIndex()
    {
        RefreshMenuReferences();
        if (mapStyleSelectables.Count == 0) return;

        int idx = Mathf.Clamp(mapStyleUiIndex, 0, mapStyleSelectables.Count - 1);
        Selectable selected = null;
        string targetMethod = GetMapStyleMethodName(mapStyleUiIndex);

        for (int i = 0; i < mapStyleSelectables.Count; i++)
        {
            Selectable selectable = mapStyleSelectables[i];
            if (selectable == null) continue;

            bool isTarget = !string.IsNullOrEmpty(targetMethod) && HasPersistentMethod(selectable, typeof(MapSwitcher), targetMethod);
            if (isTarget) selected = selectable;

            Toggle toggle = selectable.GetComponent<Toggle>();
            if (toggle != null)
                toggle.SetIsOnWithoutNotify(isTarget);

            ApplyMapStyleVisualState(selectable, isTarget);
        }

        if (selected == null)
        {
            selected = mapStyleSelectables[idx];
            Toggle fallbackToggle = selected != null ? selected.GetComponent<Toggle>() : null;
            if (fallbackToggle != null)
                fallbackToggle.SetIsOnWithoutNotify(true);
        }

        if (currentMode == ControlMode.MenuSelect && NormalizeMenuIndex(currentMenuIndex) == MenuMapStyle && EventSystem.current != null && selected != null)
            EventSystem.current.SetSelectedGameObject(selected.gameObject);
    }

    void ApplyMapStyleVisualState(Selectable selectable, bool isSelected)
    {
        if (selectable == null || selectable.targetGraphic == null) return;

        ColorBlock colors = selectable.colors;
        Color targetColor = isSelected ? colors.selectedColor : colors.normalColor;
        selectable.targetGraphic.CrossFadeColor(targetColor, 0f, true, true);
    }

    string GetMapStyleMethodName(int mapIndex)
    {
        if (mapIndex == 0) return "ShowDefault";
        if (mapIndex == 1) return "ShowSatellite";
        if (mapIndex == 2) return "ShowTerrain";
        return "";
    }

    void ApplyData()
    {
        if (waterSystemManager == null) return;

        if (waterSystemManager.dataToggles != null &&
            dataUiIndex >= 0 &&
            dataUiIndex < waterSystemManager.dataToggles.Length &&
            waterSystemManager.dataToggles[dataUiIndex] != null)
        {
            waterSystemManager.dataToggles[dataUiIndex].SetIsOnWithoutNotify(true);
        }

        waterSystemManager.OnDataToggleChanged(dataUiIndex);
        Debug.Log("[Data Apply] " + (dataUiIndex == 0 ? "Existing" : "Prediction"));
    }

    void StepDate(int step, bool isHorizontal)
    {
        if (waterSystemManager == null) return;

        if (isHorizontal)
        {
            dateUiIndex = Wrap(dateUiIndex + step, 0, 3);
            Debug.Log("[Date Focus] " + DateFocusName(dateUiIndex));
            return;
        }

        switch (dateUiIndex)
        {
            case 0:
                StepDropdown(waterSystemManager.startMonthDropdown, step);
                break;
            case 1:
                StepDropdown(waterSystemManager.startYearDropdown, step);
                waterSystemManager.UpdateDurationDropdownOptions();
                break;
            case 2:
                ToggleDurationType();
                break;
            case 3:
                StepDropdown(waterSystemManager.durationValueDropdown, step);
                break;
        }

        Debug.Log("[Date Preview] " + BuildDateSummary());
    }

    void ApplyDate()
    {
        if (waterSystemManager == null) return;

        waterSystemManager.UpdateDurationDropdownOptions();
        waterSystemManager.UpdateParamLineChart();
        Debug.Log("[Date Apply] " + BuildDateSummary());
    }

    string DateFocusName(int idx)
    {
        string[] names = { "StartMonth", "StartYear", "DurationType", "DurationValue" };
        return names[Mathf.Clamp(idx, 0, names.Length - 1)];
    }

    string BuildDateSummary()
    {
        if (waterSystemManager == null) return "No Date";

        string month = SafeDropdownText(waterSystemManager.startMonthDropdown);
        string year = SafeDropdownText(waterSystemManager.startYearDropdown);
        string durType = (waterSystemManager.durationMonthToggle != null && waterSystemManager.durationMonthToggle.isOn) ? "Month" : "Year";
        string durVal = SafeDropdownText(waterSystemManager.durationValueDropdown);

        return "Start=" + month + "-" + year + ", Type=" + durType + ", Value=" + durVal;
    }

    void StepDropdown(TMP_Dropdown dropdown, int step)
    {
        if (dropdown == null || dropdown.options == null || dropdown.options.Count == 0) return;

        int count = dropdown.options.Count;
        int next = dropdown.value + step;

        if (next < 0) next = count - 1;
        if (next >= count) next = 0;

        dropdown.value = next;
        dropdown.RefreshShownValue();
    }

    void ToggleDurationType()
    {
        if (waterSystemManager == null) return;

        bool useMonth = (waterSystemManager.durationMonthToggle != null && waterSystemManager.durationMonthToggle.isOn);
        bool nextUseMonth = !useMonth;

        if (waterSystemManager.durationMonthToggle != null)
            waterSystemManager.durationMonthToggle.SetIsOnWithoutNotify(nextUseMonth);

        if (waterSystemManager.durationYearToggle != null)
            waterSystemManager.durationYearToggle.SetIsOnWithoutNotify(!nextUseMonth);

        waterSystemManager.UpdateDurationDropdownOptions();
    }

    string SafeDropdownText(TMP_Dropdown dropdown)
    {
        if (dropdown == null || dropdown.options == null || dropdown.options.Count == 0) return "";
        if (dropdown.value < 0 || dropdown.value >= dropdown.options.Count) return "";
        return dropdown.options[dropdown.value].text;
    }

    void StepParameter(int step)
    {
        parameterUiIndex = Wrap(parameterUiIndex + step, 0, 2);
        Debug.Log("[Parameter Preview] " + ParamGroupName(parameterUiIndex));
    }

    void ApplyParameter()
    {
        if (waterSystemManager == null) return;

        if (parameterUiIndex == 0)
        {
            if (waterSystemManager.mainCategoryToggles != null &&
                waterSystemManager.mainCategoryToggles.Length > 0 &&
                waterSystemManager.mainCategoryToggles[0] != null)
            {
                waterSystemManager.mainCategoryToggles[0].SetIsOnWithoutNotify(true);
            }
            waterSystemManager.OnToggleAllChemical(true);
        }
        else if (parameterUiIndex == 1)
        {
            if (waterSystemManager.mainCategoryToggles != null &&
                waterSystemManager.mainCategoryToggles.Length > 1 &&
                waterSystemManager.mainCategoryToggles[1] != null)
            {
                waterSystemManager.mainCategoryToggles[1].SetIsOnWithoutNotify(true);
            }
            waterSystemManager.OnToggleNWQ(true);
        }
        else
        {
            if (waterSystemManager.mainCategoryToggles != null &&
                waterSystemManager.mainCategoryToggles.Length > 2 &&
                waterSystemManager.mainCategoryToggles[2] != null)
            {
                waterSystemManager.mainCategoryToggles[2].SetIsOnWithoutNotify(true);
            }
            waterSystemManager.OnToggleDOE(true);
        }

        waterSystemManager.UpdateParamLineChart();
        Debug.Log("[Parameter Apply] " + ParamGroupName(parameterUiIndex));
    }

    string ParamGroupName(int idx)
    {
        string[] names = { "ALL", "NWQ", "DOE" };
        return names[Mathf.Clamp(idx, 0, names.Length - 1)];
    }

    // =========================
    // UI / State sync
    // =========================

    void ForceRefreshLineChart()
    {
        EnsureDateUiReady();

        if (waterSystemManager != null)
            waterSystemManager.UpdateParamLineChart();
    }

    void SyncLocalStateFromUI()
    {
        if (waterSystemManager == null) return;

        currentMenuIndex = NormalizeMenuIndex(currentMenuIndex);
        mapStyleUiIndex = Mathf.Clamp(currentMapIndex, 0, 2);
        classUiIndex = GetSelectedToggleIndex(waterSystemManager.classToggles, classUiIndex);
        parameterUiIndex = GetSelectedToggleIndex(waterSystemManager.mainCategoryToggles, parameterUiIndex);

        if (!string.IsNullOrEmpty(waterSystemManager.selectedRiver))
        {
            selectedRiver = waterSystemManager.selectedRiver.ToLower();
            currentRiver = selectedRiver;
        }

        SyncRiverMenuSelectionFromName(string.IsNullOrEmpty(hoverRiver) ? selectedRiver : hoverRiver);

        if (UseMergedDataFlow())
        {
            if (!IsMergedDataSlotValid(dataUiIndex))
                dataUiIndex = GetSelectedToggleIndex(waterSystemManager.dataToggles, 0);

            dataUiIndex = Mathf.Clamp(dataUiIndex, 0, 9);
        }
        else
        {
            dataUiIndex = GetSelectedToggleIndex(waterSystemManager.dataToggles, dataUiIndex);
            dateUiIndex = Mathf.Clamp(dateUiIndex, 0, 3);
        }
    }

    int GetSelectedToggleIndex(Toggle[] toggles, int fallback)
    {
        if (toggles == null || toggles.Length == 0) return fallback;
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i] != null && toggles[i].isOn) return i;
        }
        return Mathf.Clamp(fallback, 0, toggles.Length - 1);
    }

    int Wrap(int value, int min, int max)
    {
        int range = max - min + 1;
        if (range <= 0) return min;

        while (value < min) value += range;
        while (value > max) value -= range;
        return value;
    }
}
