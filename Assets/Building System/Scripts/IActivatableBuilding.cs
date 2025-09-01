using UnityEngine;

/// <summary>
/// 定义了一个所有可被Constructable激活的建筑逻辑都应遵循的标准。
/// </summary>
public interface IActivatableBuilding
{
    /// <summary>
    /// 当建筑被正式放置后，此方法将被 Constructable 脚本调用。
    /// </summary>
    /// <param name="gridPosition">建筑所在的网格坐标</param>
    void Activate(Vector3Int gridPosition);
}