#nullable enable
using System;
using System.Reflection;
using System.Text;

namespace BorderlessGaming.Logic.NekoBoiNick
{
    /// <summary>
    /// Extension methods for <see cref="Type" />.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Searches for the public method with the specified name.
        /// </summary>
        /// <param name="type">The specified <see cref="Type" /> to get the method of.</param>
        /// <param name="methodName">The <see langword="string" /> containing the name of the public method to get.</param>
        /// <returns>
        /// A <see langword="string" /> containing the fully qualified name of the method if found;
        /// otherwise <see langword="null" />.
        /// </returns>
        public static string? GetFullyQualifiedMethodName(this Type? type, string methodName)
        {
            if (type is null || type.GetMethod(methodName) is not MethodInfo methodInfo)
            {
                return null;
            }

            return methodInfo.GetFullyQualifiedMethodName();
        }

        /// <summary>
        /// Searches for the specified method, using the specified binding constraints.
        /// </summary>
        /// <param name="type">The specified <see cref="Type" /> to get the method of.</param>
        /// <param name="methodName">The <see langword="string" /> containing the name of the public method to get.</param>
        /// <param name="bindingAttr">
        /// A bitwise combination of the enumeration values that specify how the search is conducted.
        /// -or-
        /// <see cref="BindingFlags.Default" /> to return <see langword="null" />.
        /// </param>
        /// <returns>
        /// A <see langword="string" /> containing the fully qualified name of the method if found;
        /// otherwise <see langword="null" />.
        /// </returns>
        public static string? GetFullyQualifiedMethodName(this Type type, string methodName, BindingFlags bindingAttr)
        {
            if (type.GetMethod(methodName, bindingAttr) is not MethodInfo methodInfo)
            {
                return null;
            }

            return methodInfo.GetFullyQualifiedMethodName();
        }

        /// <summary>
        /// Searches for the specified public method whose parameters match the specified argument types.
        /// </summary>
        /// <param name="type">The specified <see cref="Type" /> to get the method of.</param>
        /// <param name="methodName">The <see langword="string" /> containing the name of the public method to get.</param>
        /// <param name="types">
        /// An array of <see cref="Type" /> objects representing the number, order, and type of the parameters for the method
        /// to get.
        /// -or-
        /// An empty array of <see cref="Type" /> objects (as provided by the EmptyTypes field) to get a method that takes no
        /// parameters.
        /// </param>
        /// <returns>
        /// A <see langword="string" /> containing the fully qualified name of the method if found;
        /// otherwise <see langword="null" />.
        /// </returns>
        public static string? GetFullyQualifiedMethodName(this Type? type, string methodName, params Type[] types)
        {
            if (type is null || type.GetMethod(methodName, types) is not MethodInfo methodInfo)
            {
                return null;
            }

            return methodInfo.GetFullyQualifiedMethodName();
        }

        /// <summary>
        /// Searches for the specified method whose parameters match the specified argument types, using the specified binding
        /// constraints.
        /// </summary>
        /// <param name="type">The specified <see cref="Type" /> to get the method of.</param>
        /// <param name="methodName">The <see langword="string" /> containing the name of the public method to get.</param>
        /// <param name="bindingAttr">
        /// A bitwise combination of the enumeration values that specify how the search is conducted.
        /// -or-
        /// <see cref="BindingFlags.Default" /> to return <see langword="null" />.
        /// </param>
        /// <param name="types">
        /// An array of <see cref="Type" /> objects representing the number, order, and type of the parameters for the method
        /// to get.
        /// -or-
        /// An empty array of <see cref="Type" /> objects (as provided by the EmptyTypes field) to get a method that takes no
        /// parameters.
        /// </param>
        /// <returns>
        /// A <see langword="string" /> containing the fully qualified name of the method if found;
        /// otherwise <see langword="null" />.
        /// </returns>
        public static string? GetFullyQualifiedMethodName(this Type? type, string methodName, BindingFlags bindingAttr, Type[] types)
        {
            if (type is null || type.GetMethod(methodName, bindingAttr, types) is not MethodInfo methodInfo)
            {
                return null;
            }

            return methodInfo.GetFullyQualifiedMethodName();
        }

        /// <summary>
        /// Searches for the specified public method whose parameters match the specified generic parameter count and argument
        /// types.
        /// </summary>
        /// <param name="type">The specified <see cref="Type" /> to get the method of.</param>
        /// <param name="methodName">The <see langword="string" /> containing the name of the public method to get.</param>
        /// <param name="genericParameterCount">The number of generic type parameters of the method.</param>
        /// <param name="types">
        /// An array of <see cref="Type" /> objects representing the number, order, and type of the parameters for the method
        /// to get.
        /// -or-
        /// An empty array of <see cref="Type" /> objects (as provided by the EmptyTypes field) to get a method that takes no
        /// parameters.
        /// </param>
        /// <returns>
        /// A <see langword="string" /> containing the fully qualified name of the method if found;
        /// otherwise <see langword="null" />.
        /// </returns>
        public static string? GetFullyQualifiedMethodName(this Type? type, string methodName, int genericParameterCount, Type[] types)
        {
            if (type is null || type.GetMethod(methodName, genericParameterCount, types) is not MethodInfo methodInfo)
            {
                return null;
            }

            return methodInfo.GetFullyQualifiedMethodName();
        }
        
        /// <summary>
        /// Searches for the specified public method whose parameters match the specified argument types and modifiers.
        /// </summary>
        /// <param name="type">The specified <see cref="Type" /> to get the method of.</param>
        /// <param name="methodName">The <see langword="string" /> containing the name of the public method to get.</param>
        /// <param name="types">
        /// An array of <see cref="Type" /> objects representing the number, order, and type of the parameters for the method
        /// to get.
        /// -or-
        /// An empty array of <see cref="Type" /> objects (as provided by the EmptyTypes field) to get a method that takes no
        /// parameters.
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="ParameterModifier" /> objects representing the attributes associated with the corresponding
        /// element in the types array. To be only used when calling through COM interop, and only parameters that are passed
        /// by reference are handled. The default binder does not process this parameter.
        /// </param>
        /// <returns>
        /// A <see langword="string" /> containing the fully qualified name of the method if found;
        /// otherwise <see langword="null" />.
        /// </returns>
        public static string? GetFullyQualifiedMethodName(this Type? type, string methodName, Type[] types, ParameterModifier[]? modifiers)
        {
            if (type is null || type.GetMethod(methodName, types, modifiers) is not MethodInfo methodInfo)
            {
                return null;
            }

            return methodInfo.GetFullyQualifiedMethodName();
        }

        /// <summary>
        /// Searches for the specified public method whose parameters match the specified generic parameter count, argument
        /// types and modifiers.
        /// </summary>
        /// <param name="type">The specified <see cref="Type" /> to get the method of.</param>
        /// <param name="methodName">The <see langword="string" /> containing the name of the public method to get.</param>
        /// <param name="genericParameterCount">The number of generic type parameters of the method.</param>
        /// <param name="types">
        /// An array of <see cref="Type" /> objects representing the number, order, and type of the parameters for the method
        /// to get.
        /// -or-
        /// An empty array of <see cref="Type" /> objects (as provided by the EmptyTypes field) to get a method that takes no
        /// parameters.
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="ParameterModifier" /> objects representing the attributes associated with the corresponding
        /// element in the types array. To be only used when calling through COM interop, and only parameters that are passed
        /// by reference are handled. The default binder does not process this parameter.
        /// </param>
        /// <returns>
        /// A <see langword="string" /> containing the fully qualified name of the method if found;
        /// otherwise <see langword="null" />.
        /// </returns>
        public static string? GetFullyQualifiedMethodName(this Type? type, string methodName, int genericParameterCount, Type[] types, ParameterModifier[]? modifiers)
        {
            if (type is null || type.GetMethod(methodName, genericParameterCount, types, modifiers) is not MethodInfo methodInfo)
            {
                return null;
            }

            return methodInfo.GetFullyQualifiedMethodName();
        }

        /// <summary>
        /// Searches for the specified method whose parameters match the specified argument types and modifiers, using the
        /// specified binding constraints.
        /// </summary>
        /// <param name="type">The specified <see cref="Type" /> to get the method of.</param>
        /// <param name="methodName">The <see langword="string" /> containing the name of the public method to get.</param>
        /// <param name="bindingAttr">
        /// A bitwise combination of the enumeration values that specify how the search is conducted.
        /// -or-
        /// <see cref="BindingFlags.Default" /> to return <see langword="null" />.
        /// </param>
        /// <param name="binder">
        /// An object that defines a set of properties and enables binding, which can involve selection of an overloaded
        /// method, coercion of argument types, and invocation of a member through reflection.
        /// -or-
        /// A <see langword="null" /> reference (Nothing in Visual Basic), to use the <see cref="Type.DefaultBinder" />.
        /// </param>
        /// <param name="types">
        /// An array of <see cref="Type" /> objects representing the number, order, and type of the parameters for the method
        /// to get.
        /// -or-
        /// An empty array of <see cref="Type" /> objects (as provided by the EmptyTypes field) to get a method that takes no
        /// parameters.
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="ParameterModifier" /> objects representing the attributes associated with the corresponding
        /// element in the types array. To be only used when calling through COM interop, and only parameters that are passed
        /// by reference are handled. The default binder does not process this parameter.
        /// </param>
        /// <returns>
        /// A <see langword="string" /> containing the fully qualified name of the method if found;
        /// otherwise <see langword="null" />.
        /// </returns>
        public static string? GetFullyQualifiedMethodName(this Type? type, string methodName, BindingFlags bindingAttr, Binder? binder, Type[] types, ParameterModifier[]? modifiers)
        {
            if (type is null || type.GetMethod(methodName, bindingAttr, binder, types, modifiers) is not MethodInfo methodInfo)
            {
                return null;
            }

            return methodInfo.GetFullyQualifiedMethodName();
        }
        
        /// <summary>
        /// Searches for the specified method whose parameters match the specified generic parameter count, argument types and
        /// modifiers, using the specified binding constraints.
        /// </summary>
        /// <param name="type">The specified <see cref="Type" /> to get the method of.</param>
        /// <param name="methodName">The <see langword="string" /> containing the name of the public method to get.</param>
        /// <param name="genericParameterCount">The number of generic type parameters of the method.</param>
        /// <param name="bindingAttr">
        /// A bitwise combination of the enumeration values that specify how the search is conducted.
        /// -or-
        /// <see cref="BindingFlags.Default" /> to return <see langword="null" />.
        /// </param>
        /// <param name="binder">
        /// An object that defines a set of properties and enables binding, which can involve selection of an overloaded
        /// method, coercion of argument types, and invocation of a member through reflection.
        /// -or-
        /// A <see langword="null" /> reference (Nothing in Visual Basic), to use the <see cref="Type.DefaultBinder" />.
        /// </param>
        /// <param name="types">
        /// An array of <see cref="Type" /> objects representing the number, order, and type of the parameters for the method
        /// to get.
        /// -or-
        /// An empty array of <see cref="Type" /> objects (as provided by the EmptyTypes field) to get a method that takes no
        /// parameters.
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="ParameterModifier" /> objects representing the attributes associated with the corresponding
        /// element in the types array. To be only used when calling through COM interop, and only parameters that are passed
        /// by reference are handled. The default binder does not process this parameter.
        /// </param>
        /// <returns>
        /// A <see langword="string" /> containing the fully qualified name of the method if found;
        /// otherwise <see langword="null" />.
        /// </returns>
        public static string? GetFullyQualifiedMethodName(this Type? type, string methodName, int genericParameterCount,
                                                          BindingFlags bindingAttr, Binder? binder, Type[] types,
                                                          ParameterModifier[]? modifiers)
        {
            if (type is null || type.GetMethod(methodName, genericParameterCount, bindingAttr, binder, types, modifiers) is not MethodInfo methodInfo)
            {
                return null;
            }

            return methodInfo.GetFullyQualifiedMethodName();
        }
        
        /// <summary>
        /// Searches for the specified method whose parameters match the specified argument types and modifiers, using the
        /// specified binding constraints and the specified calling convention.
        /// </summary>
        /// <param name="type">The specified <see cref="Type" /> to get the method of.</param>
        /// <param name="methodName">The <see langword="string" /> containing the name of the public method to get.</param>
        /// <param name="bindingAttr">
        /// A bitwise combination of the enumeration values that specify how the search is conducted.
        /// -or-
        /// <see cref="BindingFlags.Default" /> to return <see langword="null" />.
        /// </param>
        /// <param name="binder">
        /// An object that defines a set of properties and enables binding, which can involve selection of an overloaded
        /// method, coercion of argument types, and invocation of a member through reflection.
        /// -or-
        /// A <see langword="null" /> reference (Nothing in Visual Basic), to use the <see cref="Type.DefaultBinder" />.
        /// </param>
        /// <param name="callingConvention">
        /// The object that specifies the set of rules to use regarding the order and layout of arguments, how the return value
        /// is passed, what registers are used for arguments, and how the stack is cleaned up.
        /// </param>
        /// <param name="types">
        /// An array of <see cref="Type" /> objects representing the number, order, and type of the parameters for the method
        /// to get.
        /// -or-
        /// An empty array of <see cref="Type" /> objects (as provided by the EmptyTypes field) to get a method that takes no
        /// parameters.
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="ParameterModifier" /> objects representing the attributes associated with the corresponding
        /// element in the types array. To be only used when calling through COM interop, and only parameters that are passed
        /// by reference are handled. The default binder does not process this parameter.
        /// </param>
        /// <returns>
        /// A <see langword="string" /> containing the fully qualified name of the method if found;
        /// otherwise <see langword="null" />.
        /// </returns>
        public static string? GetFullyQualifiedMethodName(this Type? type, string methodName, BindingFlags bindingAttr,
                                                          Binder? binder, CallingConventions callingConvention,
                                                          Type[] types, ParameterModifier[]? modifiers)
        {
            if (type is null || type.GetMethod(methodName, bindingAttr, binder, callingConvention, types, modifiers) is not MethodInfo methodInfo)
            {
                return null;
            }

            return methodInfo.GetFullyQualifiedMethodName();
        }
        

        /// <summary>
        /// Searches for the specified method whose parameters match the specified generic parameter count, argument types and
        /// modifiers, using the specified binding constraints and the specified calling convention.
        /// </summary>
        /// <param name="type">The specified <see cref="Type" /> to get the method of.</param>
        /// <param name="methodName">The <see langword="string" /> containing the name of the public method to get.</param>
        /// <param name="genericParameterCount">The number of generic type parameters of the method.</param>
        /// <param name="bindingAttr">
        /// A bitwise combination of the enumeration values that specify how the search is conducted.
        /// -or-
        /// <see cref="BindingFlags.Default" /> to return <see langword="null" />.
        /// </param>
        /// <param name="binder">
        /// An object that defines a set of properties and enables binding, which can involve selection of an overloaded
        /// method, coercion of argument types, and invocation of a member through reflection.
        /// -or-
        /// A <see langword="null" /> reference (Nothing in Visual Basic), to use the <see cref="Type.DefaultBinder" />.
        /// </param>
        /// <param name="callingConvention">
        /// The object that specifies the set of rules to use regarding the order and layout of arguments, how the return value
        /// is passed, what registers are used for arguments, and how the stack is cleaned up.
        /// </param>
        /// <param name="types">
        /// An array of <see cref="Type" /> objects representing the number, order, and type of the parameters for the method
        /// to get.
        /// -or-
        /// An empty array of <see cref="Type" /> objects (as provided by the EmptyTypes field) to get a method that takes no
        /// parameters.
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="ParameterModifier" /> objects representing the attributes associated with the corresponding
        /// element in the types array. To be only used when calling through COM interop, and only parameters that are passed
        /// by reference are handled. The default binder does not process this parameter.
        /// </param>
        /// <returns>
        /// A <see langword="string" /> containing the fully qualified name of the method if found;
        /// otherwise <see langword="null" />.
        /// </returns>
        public static string? GetFullyQualifiedMethodName(this Type? type, string methodName, int genericParameterCount,
                                                          BindingFlags bindingAttr, Binder? binder,
                                                          CallingConventions callingConvention, Type[] types,
                                                          ParameterModifier[]? modifiers)
        {
            if (type is null || type.GetMethod(methodName, genericParameterCount, bindingAttr,
                                               binder, callingConvention, types, modifiers) is not MethodInfo methodInfo)
            {
                return null;
            }

            return methodInfo.GetFullyQualifiedMethodName();
        }

        /// <summary>
        /// Returns the fully qualified name of the specified <see cref="MethodInfo" />.
        /// </summary>
        /// <param name="methodInfo">The instance of a <see cref="MethodInfo" /> to get the fully qualified name of.</param>
        /// <returns>
        /// A <see langword="string" /> containing the fully qualified name of the specified <see cref="MethodInfo" />;
        /// otherwise <see langword="null" />.
        /// </returns>
        public static string? GetFullyQualifiedMethodName(this MethodInfo? methodInfo) {
            if (methodInfo is null)
            {
                return null;
            }
            var sb = new StringBuilder();
            if (methodInfo.DeclaringType is Type declaringType)
            {
                sb.Append(declaringType.FullName).Append('.');
            }
            sb.Append(methodInfo.Name);
            if (methodInfo.IsGenericMethod)
            {
                var genericParameters = methodInfo.GetGenericArguments();
                sb.Append('<');
                for (int i = 0; i < genericParameters.Length; i++)
                {
                    sb.Append(genericParameters[i].FullName);
                    if (i < genericParameters.Length - 1)
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append('>');
            }
            sb.Append('(');
            var parameters = methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                sb.Append(parameter.ParameterType.FullName).Append(' ').Append(parameter.Name);
                if (i < parameters.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            return sb.Append("): ").Append(methodInfo.ReturnType.FullName).ToString();
        }
    }
}
