using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;

public class PiSystemBridge : MonoBehaviour
{
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

    [Header("River Selection")]
    public float riverSelectionCooldown = 0.15f;
    public float snapRadius = 1.2f;

    [Header("Debug State")]
    public ControlMode currentMode = ControlMode.MapSelect;
    public int currentMapIndex = 0;
    public int currentMenuIndex = 0;
    public string currentRiver = "";

    private UdpClient udpClient;
    private Thread receiveThread;
    private readonly object msgLock = new object();
    private string pendingMessage = null;

    private int currentSlider1Raw = 0;
    private int currentSlider2Raw = 0;
    private float lastRiverSelectTime = -10f;

    private readonly string[] menuNames = { "CLASS", "DATA", "DATE", "PARAMETER" };

    private int classIndex = 0;
    private int dataIndex = 0;
    private int dateFocusIndex = 0;
    private int paramGroupIndex = 0;

    private float lastJoyMoveTime = -10f;
    public float joyMoveCooldown = 0.22f;

    void Start()
    {
        StartReceiver();
        ApplyMapIndex();
        Debug.Log("[PiSystemBridge] Started.");
    }

    void StartReceiver()
    {
        udpClient = new UdpClient(listenPort);
        receiveThread = new Thread(ReceiveLoop);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log("[PiSystemBridge] UDP listening on port " + listenPort);
    }

    void ReceiveLoop()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);

        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string msg = Encoding.UTF8.GetString(data).Trim();

                lock (msgLock)
                {
                    pendingMessage = msg;
                }
            }
            catch
            {
                break;
            }
        }
    }

    void Update()
    {
        string msg = null;

        lock (msgLock)
        {
            if (!string.IsNullOrEmpty(pendingMessage))
            {
                msg = pendingMessage;
                pendingMessage = null;
            }
        }

        if (string.IsNullOrEmpty(msg)) return;

        HandleMessage(msg);
    }

    void HandleMessage(string msg)
    {
        Debug.Log("[Pi] " + msg);

        if (msg.StartsWith("SLIDER1:"))
        {
            if (int.TryParse(msg.Substring("SLIDER1:".Length).Trim(), out int raw))
            {
                currentSlider1Raw = raw;
                UpdateHorizontalSlider(raw);
                UpdateIntersectionAndRiver();
            }
            return;
        }

        if (msg.StartsWith("SLIDER2:"))
        {
            if (int.TryParse(msg.Substring("SLIDER2:".Length).Trim(), out int raw))
            {
                currentSlider2Raw = raw;
                UpdateVerticalSlider(raw);
                UpdateIntersectionAndRiver();
            }
            return;
        }

        if (msg.StartsWith("RIVER:"))
        {
            string riverName = msg.Substring("RIVER:".Length).Trim().ToUpper();
            SelectRiverByName(riverName);
            return;
        }

        if (msg == "JOYBTN")
        {
            HandleJoystickClick();
            return;
        }

        if (msg == "JOY:LEFT")
        {
            HandleDirectionalInput(+1, true);
            return;
        }
        if (msg == "JOY:RIGHT")
        {
            HandleDirectionalInput(-1, true);
            return;
        }
        if (msg == "JOY:UP")
        {
            HandleDirectionalInput(-1, false);
            return;
        }
        if (msg == "JOY:DOWN")
        {
            HandleDirectionalInput(+1, false);
            return;
        }

        if (msg == "ENC2:1")
        {
            Debug.Log("[ENC2] got +1, mode=" + currentMode);

            if (currentMode == ControlMode.MenuSelect)
            {
                StepMenu(+1);
                Debug.Log("[ENC2] StepMenu +1");
            }
            return;
        }

        if (msg == "ENC2:-1")
        {
            Debug.Log("[ENC2] got -1, mode=" + currentMode);

            if (currentMode == ControlMode.MenuSelect)
            {
                StepMenu(-1);
                Debug.Log("[ENC2] StepMenu -1");
            }
            return;
        }

        if (msg == "ENC3:1")
        {
            StepZoom(+1);
            return;
        }
        if (msg == "ENC3:-1")
        {
            StepZoom(-1);
            return;
        }
    }

    void HandleJoystickClick()
    {
        if (currentMode != ControlMode.MenuSelect) return;
        ConfirmCurrentMenuSelection();
    }

    void HandleDirectionalInput(int step, bool isHorizontal)
    {
        if (Time.time - lastJoyMoveTime < joyMoveCooldown) return;
        lastJoyMoveTime = Time.time;

        if (currentMode != ControlMode.MenuSelect) return;

        switch (currentMenuIndex)
        {
            case 0:
                StepClass(step);
                break;
            case 1:
                StepData(step);
                break;
            case 2:
                StepDate(step, isHorizontal);
                break;
            case 3:
                StepParameter(step);
                break;
        }
    }

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

        Debug.Log("[Map] " + currentMapIndex);
    }

    void StepMenu(int step)
    {
        currentMenuIndex = Wrap(currentMenuIndex + step, 0, 3);
        Debug.Log("[Menu] " + menuNames[currentMenuIndex]);
    }

    void ConfirmCurrentMenuSelection()
    {
        switch (currentMenuIndex)
        {
            case 0:
                ApplyClass();
                break;
            case 1:
                ApplyData();
                break;
            case 2:
                ApplyDate();
                break;
            case 3:
                ApplyParameter();
                break;
        }
    }

    void StepClass(int step)
    {
        classIndex = Wrap(classIndex + step, 0, 4);
        Debug.Log("[Class Preview] " + ClassIndexToName(classIndex));
    }

    void ApplyClass()
    {
        if (waterSystemManager == null) return;

        if (waterSystemManager.classToggles != null &&
            classIndex >= 0 &&
            classIndex < waterSystemManager.classToggles.Length &&
            waterSystemManager.classToggles[classIndex] != null)
        {
            waterSystemManager.classToggles[classIndex].SetIsOnWithoutNotify(true);
        }

        waterSystemManager.OnClassToggleChanged(classIndex);
        Debug.Log("[Class Apply] " + ClassIndexToName(classIndex));
    }

    string ClassIndexToName(int idx)
    {
        string[] names = { "All", "I", "II", "III", "IV" };
        return names[Mathf.Clamp(idx, 0, names.Length - 1)];
    }

    void StepData(int step)
    {
        dataIndex = Wrap(dataIndex + step, 0, 1);
        Debug.Log("[Data Preview] " + (dataIndex == 0 ? "Existing" : "Prediction"));
    }

    void ApplyData()
    {
        if (waterSystemManager == null) return;

        if (waterSystemManager.dataToggles != null &&
            dataIndex >= 0 &&
            dataIndex < waterSystemManager.dataToggles.Length &&
            waterSystemManager.dataToggles[dataIndex] != null)
        {
            waterSystemManager.dataToggles[dataIndex].SetIsOnWithoutNotify(true);
        }

        waterSystemManager.OnDataToggleChanged(dataIndex);
        Debug.Log("[Data Apply] " + (dataIndex == 0 ? "Existing" : "Prediction"));
    }

    void StepDate(int step, bool isHorizontal)
    {
        if (waterSystemManager == null) return;

        if (isHorizontal)
        {
            dateFocusIndex = Wrap(dateFocusIndex + step, 0, 3);
            Debug.Log("[Date Focus] " + DateFocusName(dateFocusIndex));
            return;
        }

        switch (dateFocusIndex)
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

        return $"Start={month}-{year}, Type={durType}, Value={durVal}";
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
        paramGroupIndex = Wrap(paramGroupIndex + step, 0, 2);
        Debug.Log("[Parameter Preview] " + ParamGroupName(paramGroupIndex));
    }

    void ApplyParameter()
    {
        if (waterSystemManager == null) return;

        if (paramGroupIndex == 0)
        {
            if (waterSystemManager.mainCategoryToggles != null &&
                waterSystemManager.mainCategoryToggles.Length > 0 &&
                waterSystemManager.mainCategoryToggles[0] != null)
            {
                waterSystemManager.mainCategoryToggles[0].SetIsOnWithoutNotify(true);
            }
            waterSystemManager.OnToggleAllChemical(true);
        }
        else if (paramGroupIndex == 1)
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
        Debug.Log("[Parameter Apply] " + ParamGroupName(paramGroupIndex));
    }

    string ParamGroupName(int idx)
    {
        string[] names = { "ALL", "NWQ", "DOE" };
        return names[Mathf.Clamp(idx, 0, names.Length - 1)];
    }

    void StepZoom(int step)
    {
        if (mapRoot == null) return;

        float factor = (step > 0) ? 1.1f : 0.9f;
        Vector3 next = mapRoot.localScale * factor;

        next.x = Mathf.Clamp(next.x, minScale, maxScale);
        next.y = Mathf.Clamp(next.y, minScale, maxScale);
        next.z = Mathf.Clamp(next.z, minScale, maxScale);

        mapRoot.localScale = next;
        Debug.Log("[Zoom] " + next.x.ToString("F2"));
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
        if (sliderXContainer == null || sliderYContainer == null || mapRoot == null) return;

        Vector3 intersection = new Vector3(
            sliderXContainer.position.x,
            sliderYContainer.position.y,
            sliderXContainer.position.z
        );

        if (intersectionMarker != null)
            intersectionMarker.position = intersection;

        if (Time.time - lastRiverSelectTime < riverSelectionCooldown) return;

        Collider[] hits = Physics.OverlapSphere(intersection, snapRadius);
        Collider bestHit = null;
        float bestDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            bool isRiverLabelTag = hit.CompareTag("RiverLabel");
            bool isRiverLabelLayer = hit.gameObject.layer == LayerMask.NameToLayer("RiverLabel");

            if (!isRiverLabelTag && !isRiverLabelLayer) continue;

            float d = Vector3.Distance(intersection, hit.bounds.center);
            if (d < bestDist)
            {
                bestDist = d;
                bestHit = hit;
            }
        }

        if (bestHit == null) return;

        string riverName = LabelNameToRiverName(bestHit.name);
        if (string.IsNullOrEmpty(riverName)) return;

        SelectRiverByName(riverName);
    }

    void SelectRiverByName(string riverName)
    {
        if (string.IsNullOrEmpty(riverName)) return;
        if (riverName == currentRiver && Time.time - lastRiverSelectTime < riverSelectionCooldown) return;

        currentRiver = riverName;
        lastRiverSelectTime = Time.time;

        HighlightRiverByName(currentRiver);

        if (waterSystemManager != null)
            waterSystemManager.SelectRiver(currentRiver);

        currentMode = ControlMode.MenuSelect;
        Debug.Log("[River Selected] " + currentRiver);
        Debug.Log("[Mode] Enter MenuSelect after river select");
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

    int Wrap(int value, int min, int max)
    {
        int range = max - min + 1;
        if (range <= 0) return min;

        while (value < min) value += range;
        while (value > max) value -= range;
        return value;
    }

    void OnApplicationQuit()
    {
        try
        {
            receiveThread?.Abort();
        }
        catch { }

        udpClient?.Close();
    }
}
