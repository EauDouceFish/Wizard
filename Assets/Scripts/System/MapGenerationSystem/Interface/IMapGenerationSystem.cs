using QFramework;

public interface IMapGenerationSystem : ISystem
{
    /// <summary>
    /// 执行管线，具体数据从MapModel获取
    /// </summary>
    void ExecuteGenerationPipeline();
}