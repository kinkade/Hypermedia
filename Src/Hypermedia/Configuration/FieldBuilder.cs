﻿using System;
using System.Linq.Expressions;
using Hypermedia.Metadata;
using Hypermedia.Metadata.Runtime;

namespace Hypermedia.Configuration
{
    public sealed class FieldBuilder<T> : IContractBuilder<T>
    {
        readonly IContractBuilder<T> _builder;
        string _name;
        FieldOptions _options = FieldOptions.Default;
        IFieldAccessor _fieldAccessor;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="builder">The parent builder.</param>
        /// <param name="name">The name of the field.</param>
        internal FieldBuilder(IContractBuilder<T> builder, string name)
        {
            _builder = builder;
            _name = name;
        }

        /// <summary>
        /// Build a resource contract resolver with the known types.
        /// </summary>
        /// <returns>The resource contract resolver that is aware of the types that were configured through the builder.</returns>
        public IContractResolver Build()
        {
            return _builder.Build();
        }

        /// <summary>
        /// Create the runtime field.
        /// </summary>
        /// <returns>The field.</returns>
        internal RuntimeField<T> CreateRuntimeField()
        {
            var accessor = _fieldAccessor;

            if (accessor == null)
            {
                accessor = RuntimeFieldAccessor.From<T>(_name);
            }
            
            return new RuntimeField<T>(_name, accessor.ValueType, accessor, _options);
        }

        /// <summary>
        /// Returns a Resource Builder for a resource type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the resource to return the builder for.</typeparam>
        /// <param name="discovery">The type discovery mechanism.</param>
        /// <returns>The resource builder to configure.</returns>
        public ContractBuilder<TEntity> With<TEntity>(ITypeDiscovery discovery)
        {
            return _builder.With<TEntity>(discovery);
        }

        /// <summary>
        /// Returns a field.
        /// </summary>
        /// <param name="name">The name of the field to return.</param>
        /// <returns>The field builder build the field.</returns>
        public FieldBuilder<T> Field(string name)
        {
            return _builder.Field(name);
        }

        /// <summary>
        /// Returns a BelongsTo relationship.
        /// </summary>
        /// <param name="name">The name of the relationship to return.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        public RelationshipBuilder<T> BelongsTo<TOther>(string name)
        {
            return _builder.BelongsTo<TOther>(name);
        }

        /// <summary>
        /// Returns a HasMany relationship.
        /// </summary>
        /// <param name="name">The name of the relationship to return.</param>
        /// <returns>The relationship builder build the relationship.</returns>
        public RelationshipBuilder<T> HasMany<TOther>(string name)
        {
            return _builder.HasMany<TOther>(name);
        }

        /// <summary>
        /// Sets the field accessor for the given field.
        /// </summary>
        /// <param name="accessor">The field accessor for the given field.</param>
        /// <returns>The field builder to continue building on.</returns>
        public FieldBuilder<T> Accessor(IFieldAccessor accessor)
        {
            _fieldAccessor = accessor;

            return this;
        }

        /// <summary>
        /// Sets the property that the field is to be mapped to.
        /// </summary>
        /// <param name="property">The name of the property that the field is mapped to.</param>
        /// <returns>The field builder to continue building on.</returns>
        public FieldBuilder<T> From(string property)
        {
            _fieldAccessor = RuntimeFieldAccessor.From<T>(property);

            return this;
        }

        /// <summary>
        /// Renames the field.
        /// </summary>
        /// <param name="name">The new name to apply to the field.</param>
        /// <returns>The field builder to continue building on.</returns>
        /// <remarks>If the mapping property has not been set, the current name is set
        /// as the mapping property and then the new name is applied.</remarks>
        public FieldBuilder<T> Rename(string name)
        {
            if (_fieldAccessor == null)
            {
                _fieldAccessor = RuntimeFieldAccessor.From<T>(_name);
            }

            _name = name;

            return this;
        }

        /// <summary>
        /// Sets the given options for the field.
        /// </summary>
        /// <param name="options">The list of options to set.</param>
        /// <param name="setOptionOn">true if the options are to be set, false if not.</param>
        /// <returns>The field builder to continue building on.</returns>
        public FieldBuilder<T> Options(FieldOptions options, bool setOptionOn = true)
        {
            if (setOptionOn)
            {
                _options |= options;
            }
            else
            {
                _options &= ~(options);
            }

            return this;
        }

        /// <summary>
        /// Defines whether the given field is the primary ID field.
        /// </summary>
        /// <param name="value">true if the field is the primary ID field, false if not.</param>
        /// <returns>The field builder to continue building on.</returns>
        public FieldBuilder<T> Id(bool value = true)
        {
            return Options(FieldOptions.Id, value);
        }

        /// <summary>
        /// Defines whether the given field is to be ignored.
        /// </summary>
        /// <param name="value">true to ignore the field, false if not.</param>
        /// <returns>The field builder to continue building on.</returns>
        public FieldBuilder<T> Ignore(bool value = true)
        {
            return Options(FieldOptions.CanSerialize, value == false).Options(FieldOptions.CanDeserialize, value == false);
        }

        /// <summary>
        /// Defines the field as being readonly.
        /// </summary>
        /// <returns>The field builder to continue building on.</returns>
        public FieldBuilder<T> ReadOnly()
        {
            return Options(FieldOptions.CanSerialize, true).Options(FieldOptions.CanDeserialize, false);
        }

        /// <summary>
        /// Defines the field as being write-only.
        /// </summary>
        /// <returns>The field builder to continue building on.</returns>
        public FieldBuilder<T> WriteOnly()
        {
            return Options(FieldOptions.CanSerialize, false).Options(FieldOptions.CanDeserialize, true);
        }

        /// <summary>
        /// Sets the field as a calculated field.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="expression">The expression to return the calculated value for the field.</param>
        /// <returns>The field building to continue building on.</returns>
        public CalculatedFieldBuilder<T, TValue> Calculated<TValue>(Func<T, TValue> expression)
        {
            return new CalculatedFieldBuilder<T, TValue>(this, expression);
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        internal string Name
        {
            get { return _name; }
        }
    }
}
