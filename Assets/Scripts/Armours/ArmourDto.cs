using System;
using System.Collections.Generic;

/// <summary>
/// armours.json のルートオブジェクト。
/// </summary>
[Serializable]
public class ArmourDatabaseJson
{
    public List<ArmourDto> armours = new List<ArmourDto>();
}

/// <summary>
/// JSON 上の防具 1 エントリに対応するフラットな DTO。
/// </summary>
[Serializable]
public class ArmourDto
{
    public string       armourName;
    public int          armourRating;
    public List<string> traits = new List<string>();
}
