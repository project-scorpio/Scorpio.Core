namespace Scorpio.DependencyInjection
{
    /// <summary>
    /// 作用域依赖注入标记接口，继承自 <see cref="IDependency"/>
    /// </summary>
    /// <remarks>
    /// 实现此接口的所有类都会自动注册到依赖注入容器中作为作用域（Scoped）服务
    /// </remarks>
    public interface IScopedDependency : IDependency
    {

    }
}
