using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Scorpio.DynamicProxy
{
    /// <summary>
    /// 方法调用接口，提供方法拦截时的调用上下文信息
    /// </summary>
    /// <remarks>
    /// 此接口封装了被拦截方法的所有相关信息，包括参数、目标对象、方法元数据等，供拦截器使用
    /// </remarks>
    public interface IMethodInvocation
    {
        /// <summary>
        /// 获取方法调用的参数数组
        /// </summary>
        /// <value>包含所有方法参数的对象数组 <see cref="object"/>[]</value>
        object[] Arguments { get; }

        /// <summary>
        /// 获取方法参数的只读字典，以参数名为键，参数值为值
        /// </summary>
        /// <value>参数名称与参数值的只读字典映射 <see cref="IReadOnlyDictionary{String, Object}"/></value>
        IReadOnlyDictionary<string, object> ArgumentsDictionary { get; }

        /// <summary>
        /// 获取方法的泛型参数类型数组
        /// </summary>
        /// <value>泛型参数类型数组 <see cref="Type"/>[]</value>
        Type[] GenericArguments { get; }

        /// <summary>
        /// 获取被拦截方法所属的目标对象实例
        /// </summary>
        /// <value>目标对象实例</value>
        object TargetObject { get; }

        /// <summary>
        /// 获取被拦截的方法元数据信息
        /// </summary>
        /// <value>方法元数据信息 <see cref="MethodInfo"/></value>
        MethodInfo Method { get; }

        /// <summary>
        /// 获取或设置方法调用的返回值
        /// </summary>
        /// <value>方法执行后的返回值</value>
        object ReturnValue { get; set; }

        /// <summary>
        /// 异步继续执行被拦截的方法调用
        /// </summary>
        /// <returns>表示异步操作的任务 <see cref="Task"/></returns>
        Task ProceedAsync();
    }
}
