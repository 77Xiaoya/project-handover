using System.Collections.Generic;

// 文件名必须严格为 waterQualityP.cs
public class waterQualityP 
{
    public int ID;
    public float LATITUDE;
    public float LONGTITUD;
    public string RIVER;
    public float SMPDAT;
    public int YEAR;
    public string CLASS;
    
    // 使用字典存储所有化学参数，Key是参数名(如"DO")，Value是数值
    public Dictionary<string, float> ChemicalData = new Dictionary<string, float>();
}