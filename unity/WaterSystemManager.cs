using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using XCharts.Runtime;
using System;
using System.Globalization;

public class WaterSystemManager : MonoBehaviour
{
    public static WaterSystemManager Instance;

    [Header("UI 容器控制")]
    public GameObject menuContainer;       
    public GameObject dataMenuContainer;   

    [Header("Step 1: Class 选择 (单选组)")]
    public Toggle[] classToggles;          // All, I, II, III, IV

    [Header("Step 2: 数据类型与时间范围")]
    public Toggle[] dataToggles;           // Existing, Prediction
    public TMP_Dropdown startMonthDropdown;
    public TMP_Dropdown startYearDropdown;
    public Toggle durationMonthToggle;
    public Toggle durationYearToggle;
    public TMP_Dropdown durationValueDropdown;

    [Header("Step 3: 参数分类与具体参数")]
    public Toggle[] mainCategoryToggles;   // All Chemical, NWQ, DOE
    public Toggle[] allParamToggles;       // DO, BOD, pH 等 26 个具体参数

    private readonly string[] listDOE = { "DO", "BOD", "AN", "SS", "COD", "pH" };
    private readonly string[] listNWQ = { "DO", "BOD", "AN", "SS", "COD", "pH", "COND", "SAL", "TEMP", "TUR", "Coliform", "DS", "E-coli" };
    private readonly string[] listALL = { "NO3", "Cl", "PO4", "As", "Hg", "Cd", "Cr", "Pb", "Zn", "Ca", "Fe", "K", "Mg", "Na" };
    
    private readonly Dictionary<string, string> paramHeaderMap = new Dictionary<string, string>()
    {
        { "DO", "DO (mg/l)" },
        { "BOD", "BOD (mg/l)" },
        { "COD", "COD (mg/l)" },
        { "SS", "SS (mg/l)" },
        { "pH", "pH (unit)" },
        { "AN", "NH3-NL (mg/l)" },
        { "TEMP", "TEMP (Deg)" },
        { "COND", "COND (uS)" },
        { "SAL", "SAL (ppt)" },
        { "TUR", "TUR (NTU)" },
        { "DS", "DS (mg/l)" },
        { "E-coli", "E-coli (cfu/100ml)" },
        { "Coliform", "Coliform (cfu/100ml)" },
        { "NO3", "NO3-(mg/l)" },  
        { "Cl", "Cl-(mg/l)" },
        { "PO4", "PO4-(mg/l)" },
        { "As", "As (mg/l)" },
        { "Hg", "Hg (mg/l)" },
        { "Cd", "Cd (mg/l)" },
        { "Cr", "Cr (mg/l)" },
        { "Pb", "Pb (mg/l)" },
        { "Zn", "Zn (mg/l)" },
        { "Ca", "Ca (mg/l)" },
        { "Fe", "Fe (mg/l)" },
        { "K", "K (mg/l)" },
        { "Mg", "Mg (mg/l)" },
        { "Na", "Na (mg/l)" }
    };

    [Header("图表渲染组件")]
    public BarChart classBarChart;   
    public LineChart paramLineChart; 

    [Header("当前状态 (只读观察)")]
    public string selectedRiver = "";
    public string selectedDataType = "Existing";
    public string selectedClass = "All";
    public List<string> activeParams = new List<string>();

    void Awake() { Instance = this; }

    void Start()
    {
        if (menuContainer != null) menuContainer.SetActive(false);
        if (dataMenuContainer != null) dataMenuContainer.SetActive(false);
    }

    public void SelectRiver(string riverName)
    {
        selectedRiver = riverName.ToUpper();
        if (menuContainer != null) menuContainer.SetActive(true);
        if (dataMenuContainer != null) dataMenuContainer.SetActive(true);

        activeParams.Clear(); 
        if (paramLineChart != null) paramLineChart.gameObject.SetActive(false);

        if (classToggles.Length > 0 && classToggles[0] != null) 
        {
            classToggles[0].SetIsOnWithoutNotify(true); 
            ManualToggleMute(classToggles, 0);
        }
        if (dataToggles.Length > 0 && dataToggles[0] != null) 
        {
            dataToggles[0].SetIsOnWithoutNotify(true); 
            ManualToggleMute(dataToggles, 0);
        }

        ToggleGroup(mainCategoryToggles, false); 
        ToggleGroup(allParamToggles, false);     

        selectedClass = "All";
        selectedDataType = "Existing";

        if (classBarChart != null) classBarChart.gameObject.SetActive(true);
        UpdateClassBarChart();
        
        RefreshYearOptions();
    }

    private void ToggleGroup(Toggle[] group, bool state)
    {
        if (group == null) return;
        foreach (var t in group) if (t != null) t.SetIsOnWithoutNotify(state);
    }

    public void OnDurationTypeChanged(bool isOn)
    {
        if (!isOn) return; 
        UpdateDurationDropdownOptions();
    }

    public void RefreshYearOptions()
    {
        if (startYearDropdown == null) return;
        
        startYearDropdown.ClearOptions();
        List<string> years = new List<string>();

        if (selectedDataType == "Existing")
        {
            for (int y = 2012; y <= 2018; y++) years.Add(y.ToString());
        }
        else 
        {
            for (int y = 2019; y <= 2023; y++) years.Add(y.ToString());
        }

        startYearDropdown.AddOptions(years);
        startYearDropdown.value = 0; 
        startYearDropdown.RefreshShownValue();
        
        UpdateDurationDropdownOptions(); 
    }

    public void UpdateDurationDropdownOptions()
    {
        if (durationValueDropdown == null || startYearDropdown == null || startYearDropdown.options.Count == 0) return;

        durationValueDropdown.ClearOptions();
        List<string> newOptions = new List<string>();

        if (durationMonthToggle != null && durationMonthToggle.isOn)
        {
            for (int i = 1; i <= 12; i++) newOptions.Add(i.ToString());
        }
        else if (durationYearToggle != null && durationYearToggle.isOn)
        {
            int currentStartYear = int.Parse(startYearDropdown.options[startYearDropdown.value].text);
            int maxYearLimit = (selectedDataType == "Existing") ? 2018 : 2023; 
            int maxAllowedYears = maxYearLimit - currentStartYear + 1;
            int finalLimit = Mathf.Min(5, maxAllowedYears);

            for (int i = 1; i <= finalLimit; i++) newOptions.Add(i.ToString());
        }

        durationValueDropdown.AddOptions(newOptions);
        
        if (durationValueDropdown.value >= newOptions.Count)
        {
            durationValueDropdown.value = Mathf.Max(0, newOptions.Count - 1);
        }
        
        durationValueDropdown.RefreshShownValue();
    }

    public void OnClassToggleChanged(int index)
    {
        if (!classToggles[index].isOn) return;
        ManualToggleMute(classToggles, index);

        string[] classNames = { "All", "I", "II", "III", "IV" };
        selectedClass = classNames[index];
        UpdateClassBarChart();
    }

    public void OnDataToggleChanged(int index)
    {
        if (!dataToggles[index].isOn) return;
        ManualToggleMute(dataToggles, index);

        selectedDataType = (index == 0) ? "Existing" : "Prediction";
        
        RefreshYearOptions();
        UpdateParamLineChart();
    }

    private void ManualToggleMute(Toggle[] toggleArray, int keepIndex)
    {
        for (int i = 0; i < toggleArray.Length; i++)
        {
            if (i != keepIndex && toggleArray[i] != null) 
                toggleArray[i].SetIsOnWithoutNotify(false); 
        }
    }

    public void SelectParameterGroup(string groupType, bool isOn)
    {
        if (!isOn) return;
        activeParams.Clear();
        foreach (var t in allParamToggles)
        {
            if (t == null) continue;
            bool shouldBeOn = false;
            string pName = t.name;

            if (groupType == "ALL" && listALL.Contains(pName)) shouldBeOn = true;
            else if (groupType == "NWQ" && listNWQ.Contains(pName)) shouldBeOn = true;
            else if (groupType == "DOE" && listDOE.Contains(pName)) shouldBeOn = true;

            t.SetIsOnWithoutNotify(shouldBeOn);
            if (shouldBeOn) activeParams.Add(pName);
        }
        UpdateParamLineChart();
    }

    public void OnSingleParameterChanged(Toggle t)
    {
        if (t == null) return;
        
        string pName = t.name;
        
        if (t.isOn && !activeParams.Contains(pName)) 
            activeParams.Add(pName);
        else if (!t.isOn && activeParams.Contains(pName)) 
            activeParams.Remove(pName);

        if (activeParams.Count == 0)
        {
            if (paramLineChart != null) paramLineChart.gameObject.SetActive(false);
            return;
        }

        UpdateParamLineChart();
    }

    public void UpdateClassBarChart()
    {
        if (classBarChart == null || string.IsNullOrEmpty(selectedRiver)) return;

        classBarChart.RemoveAllSerie(); 
        var xAxis = classBarChart.GetOrAddChartComponent<XAxis>();
        if (xAxis != null) xAxis.ClearData(); 

        var legend = classBarChart.GetOrAddChartComponent<Legend>();
        legend.show = true;
        legend.orient = Orient.Vertical;

        string path = "WaterData/" + selectedRiver + "_Existing";
        TextAsset csvData = Resources.Load<TextAsset>(path);
        if (csvData == null) return;

        var title = classBarChart.GetOrAddChartComponent<Title>();
        title.text = "Class Distribution - " + selectedRiver;

        string[] lines = csvData.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var stats = new Dictionary<string, Dictionary<string, int>>();
        var years = new string[] { "2012", "2013", "2014", "2015", "2016", "2017", "2018" };
        
        foreach (var y in years) {
            classBarChart.AddXAxisData(y);
            stats[y] = new Dictionary<string, int> { {"I",0}, {"II",0}, {"III",0}, {"IV",0} };
        }

        for (int i = 1; i < lines.Length; i++) {
            string[] row = lines[i].Split(',');
            if (row.Length < 47) continue; 
            
            string fullDate = row[7].Trim(); 
            string year = fullDate.Length >= 4 ? fullDate.Substring(0, 4) : ""; 
            string clvl = row[46].Trim(); 
            
            if (stats.ContainsKey(year) && stats[year].ContainsKey(clvl)) {
                stats[year][clvl]++;
            }
        }

        string[] classesToDraw = (selectedClass == "All") ? new string[]{"I","II","III","IV"} : new string[]{selectedClass};
        
        for (int i = 0; i < classesToDraw.Length; i++) {
            string className = classesToDraw[i];
            classBarChart.AddSerie<Bar>(className); 
            
            foreach (var y in years) {
                classBarChart.AddData(i, stats[y][className]);
            }
        }
    }

 public void UpdateParamLineChart()
    {
        if (paramLineChart == null || string.IsNullOrEmpty(selectedRiver) || activeParams.Count == 0) 
        {
            if (paramLineChart != null) paramLineChart.gameObject.SetActive(false);
            return;
        }
        paramLineChart.gameObject.SetActive(true);

        int startYear = int.Parse(startYearDropdown.options[startYearDropdown.value].text);
        int startMonth = startMonthDropdown.value + 1;
        int durationVal = int.Parse(durationValueDropdown.options[durationValueDropdown.value].text);
        DateTime startDate = new DateTime(startYear, startMonth, 1);
        DateTime endDate = durationMonthToggle.isOn ? startDate.AddMonths(durationVal - 1) : new DateTime(startYear + durationVal - 1, 12, 31);
        if (durationMonthToggle.isOn) endDate = new DateTime(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));

        string path = "WaterData/" + selectedRiver + (selectedDataType == "Existing" ? "_Average" : "_Prediction");
        TextAsset csvData = Resources.Load<TextAsset>(path);
        if (csvData == null) 
        {
            Debug.LogError($"[数据缺失] 找不到文件: Assets/Resources/{path}.csv，请检查大小写或文件是否存在。");
            return;
        }

        // ==========================================
        // 核心步骤 1：组件清理与网格搭建
        // ==========================================
        paramLineChart.RemoveAllSerie();

        while (paramLineChart.RemoveChartComponent<GridCoord>()) { }
        while (paramLineChart.RemoveChartComponent<XAxis>()) { }
        while (paramLineChart.RemoveChartComponent<YAxis>()) { }

        int count = activeParams.Count;
        float gap = 0.04f; 
        float totalTopMargin = 0.08f;
        float totalBottomMargin = 0.08f;
        float gridHeight = (1f - totalTopMargin - totalBottomMargin - (count - 1) * gap) / count;

        for (int i = 0; i < count; i++)
        {
            var grid = paramLineChart.AddChartComponent<GridCoord>();
            grid.left = 0.15f;  
            grid.right = 0.05f;
            grid.top = totalTopMargin + i * (gridHeight + gap);
            grid.bottom = 1f - (grid.top + gridHeight);

            var xAxis = paramLineChart.AddChartComponent<XAxis>();
            xAxis.gridIndex = i; 
            xAxis.type = Axis.AxisType.Category; 
            xAxis.data.Clear(); 

            if (i == count - 1) 
            {
                xAxis.axisLabel.show = true;
                xAxis.axisLabel.textStyle.fontSize = 10;
            } 
            else 
            {
                xAxis.axisLabel.show = false;
            }

            var yAxis = paramLineChart.AddChartComponent<YAxis>();
            yAxis.gridIndex = i; 
            yAxis.type = Axis.AxisType.Value;
            yAxis.splitNumber = 2; 
            yAxis.axisLabel.textStyle.fontSize = 10; 
            yAxis.axisLabel.numericFormatter = "F1"; 
            
            string uiName = activeParams[i];
            yAxis.axisName.name = paramHeaderMap.ContainsKey(uiName) ? paramHeaderMap[uiName] : uiName;
            yAxis.axisName.show = true;

            var serie = paramLineChart.AddSerie<Line>(uiName);
            serie.xAxisIndex = i;
            serie.yAxisIndex = i;
        }

      // ==========================================
        // 核心步骤 2：CSV数据解析与注入 (万能日期解析 + 严格过滤版)
        // ==========================================
        string[] lines = csvData.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        string[] headers = lines[0].Split(',');
        
        int[] colIndices = new int[activeParams.Count];
        for (int s = 0; s < activeParams.Count; s++) {
            string target = paramHeaderMap.ContainsKey(activeParams[s]) ? paramHeaderMap[activeParams[s]] : activeParams[s];
            colIndices[s] = Array.FindIndex(headers, h => h.Trim().Equals(target, StringComparison.OrdinalIgnoreCase));
        }

        int dateColumnIndex = Array.FindIndex(headers, h => h.Trim().Equals("SMP-DAT", StringComparison.OrdinalIgnoreCase));
        if (dateColumnIndex == -1)
        {
            Debug.LogError($"[致命错误] CSV未找到 SMP-DAT 表头，请检查文件。");
            return;
        }

        // 打印 UI 传递过来的真实边界时间，方便核对为什么 Prediction 没有被过滤
        Debug.Log($"[过滤条件生效] 开始时间: {startDate:yyyy-MM-dd}, 结束时间: {endDate:yyyy-MM-dd}");

        int injectedCount = 0; 
        bool loggedError = false;

        // 覆盖世界上绝大多数常见及异常格式的组合
        string[] expectedFormats = {
            "dd/MM/yyyy", "d/M/yyyy", "MM/dd/yyyy", "M/d/yyyy", 
            "yyyy-MM-dd", "yyyy/MM/dd", "dd-MM-yyyy", 
            "yyyy", "dd/MM/yyyy HH:mm:ss", "dd-MMM-yy", "MMM-yy"
        };

        for (int i = 1; i < lines.Length; i++) {
            string[] row = lines[i].Split(',');
            if (row.Length <= dateColumnIndex) continue;

            string dateStr = row[dateColumnIndex].Trim();

            // 核心修复：强制多重格式解析，无视操作系统的时区和格式设置
            if (DateTime.TryParseExact(dateStr, expectedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime rowDate) 
                || DateTime.TryParse(dateStr, out rowDate)) 
            {
                // 严格拦截：只有在开始和结束时间之内的数据才会绘制
                if (rowDate >= startDate && rowDate <= endDate) 
                {
                    // 修复标签堆叠：确保同一年的数据能看出月份递进，避免满屏都是 "2019"
                    string dateLabel = durationMonthToggle.isOn ? rowDate.ToString("yy-MMM") : rowDate.ToString("yyyy-MM");
                    
                    for (int s = 0; s < activeParams.Count; s++) {
                        paramLineChart.AddXAxisData(dateLabel, s); 
                        
                        float val = 0;
                        if (colIndices[s] != -1 && colIndices[s] < row.Length) {
                            float.TryParse(row[colIndices[s]].Trim(), out val);
                        }
                        paramLineChart.AddData(s, val);
                    }
                    injectedCount++;
                }
            }
            else
            {
                if (!loggedError) 
                {
                    Debug.LogError($"[日期解析彻底失败] 字符串为 '{dateStr}'。如果你看到此报错，说明 CSV 内存在非标准的脏数据。");
                    loggedError = true;
                }
            }
        }
        
        Debug.Log($"[渲染完成] 共 {injectedCount} 条数据通过时间过滤并注入。");

        // ==========================================
        // 核心步骤 3：强制执行重新渲染
        // ==========================================
        
paramLineChart.RefreshChart();
    }

    public void OnToggleAllChemical(bool isOn) { SelectParameterGroup("ALL", isOn); }
    public void OnToggleNWQ(bool isOn) { SelectParameterGroup("NWQ", isOn); }
    public void OnToggleDOE(bool isOn) { SelectParameterGroup("DOE", isOn); }
}