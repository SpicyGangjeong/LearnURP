using Unity;

public class CMyTools
{
    #region Convert
    public static T StringToType<T>(string str)
    {
        return (T)(object)str;
    }
    public static string TypeToString<T>(T pValue)
    {
        return (string)(object)pValue;
    }
    #endregion

}