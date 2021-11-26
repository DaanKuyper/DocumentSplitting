namespace WobModels;

public class ApiResultDataList
{
  public string? service { get; set; }

  public int? totalcount { get; set; }

  public List<ApiResultData>? results { get; set; }
}
