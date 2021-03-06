﻿using System;
using System.Diagnostics;
using System.Reflection;

namespace Hypermedia.Metadata.Runtime
{
    [DebuggerDisplay("{Name}")]
    internal class RuntimeField : IField
    {
        readonly string _name;
        readonly Type _clrType;
        readonly IFieldAccessor _accessor;
        readonly FieldOptions _options;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="clrType">The CLR type for the field.</param>
        /// <param name="accessor">The field accessor.</param>
        /// <param name="options">The field options.</param>
        internal RuntimeField(string name, Type clrType, IFieldAccessor accessor, FieldOptions options)
        {
            _name = name;
            _clrType = clrType;
            _accessor = accessor;
            _options = options;
        }

        /// <summary>
        /// Creates a runtime field from a property info.
        /// </summary>
        /// <param name="propertyInfo">The property info to create the runtime field for.</param>
        /// <returns>The runtime field that wraps the given property info.</returns>
        internal static RuntimeField CreateRuntimeField(PropertyInfo propertyInfo)
        {
            return new RuntimeField(
                propertyInfo.Name, 
                propertyInfo.PropertyType, 
                new RuntimeFieldAccessor(propertyInfo), 
                CreateDefaultOptions(propertyInfo));
        }

        /// <summary>
        /// Creates the default options for the property info.
        /// </summary>
        /// <param name="propertyInfo">The property info to create the default options for.</param>
        /// <returns>The field options for the given property info.</returns>
        internal static FieldOptions CreateDefaultOptions(PropertyInfo propertyInfo)
        {
            var options = FieldOptions.None;

            if (propertyInfo.CanWrite == false)
            {
                options = options | FieldOptions.CanDeserialize;
            }

            return options;
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the CLR type that the member maps to.
        /// </summary>
        public Type ClrType
        {
            get { return _clrType; }
        }

        /// <summary>
        /// Gets the field accessor.
        /// </summary>
        public IFieldAccessor Accessor
        {
            get { return _accessor; }
        }

        /// <summary>
        /// Gets the list of options for the field.
        /// </summary>
        public FieldOptions Options
        {
            get { return _options; }
        }
    }

    internal sealed class RuntimeField<T> : RuntimeField
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="clrType">The CLR type for the field.</param>
        /// <param name="accessor">The field accessor.</param>
        /// <param name="options">The field options.</param>
        internal RuntimeField(string name, Type clrType, IFieldAccessor accessor, FieldOptions options) : base(name, clrType, accessor, options) { }

        /// <summary>
        /// Creates a runtime field from a property info.
        /// </summary>
        /// <param name="propertyInfo">The property info to create the runtime field for.</param>
        /// <returns>The runtime field that wraps the given property info.</returns>
        internal new static RuntimeField<T> CreateRuntimeField(PropertyInfo propertyInfo)
        {
            return new RuntimeField<T>(
                propertyInfo.Name,
                propertyInfo.PropertyType,
                new RuntimeFieldAccessor(propertyInfo),
                CreateDefaultOptions(propertyInfo));
        }
    }
}
