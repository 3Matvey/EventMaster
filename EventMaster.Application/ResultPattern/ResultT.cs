﻿namespace EventMaster.Application.ResultPattern
{
    public sealed class Result<TValue> : Result
    {
        private readonly TValue? _value;

        private Result(TValue value) 
            : base()
        {
            _value = value;
        }

        private Result(Error error) 
            : base(error)
        {
            _value = default;
        }

        public TValue Value 
            => IsSuccess 
            ? _value! 
            : throw new InvalidOperationException("Value cannot be accessed when IsSuccess is false");

        /// <summary>
        /// Implicitly converts an <see cref="Error"/> to a <see cref="Result{TValue}"/> with a failed result.
        /// </summary>
        /// <param name="error">The error.</param>
        public static implicit operator Result<TValue>(Error error) =>
            new(error);

        /// <summary>
        /// Implicitly converts a value of type <typeparamref name="TValue"/> to a <see cref="Result{TValue}"/> with a successful result.
        /// </summary>
        /// <param name="value">The value.</param>
        public static implicit operator Result<TValue>(TValue value) =>
            new(value);

        /// <summary>
        /// Creates a new <see cref="Result{TValue}"/> with a successful result and a value.
        /// </summary>
        /// <param name="value">The value.</param>
        public static Result<TValue> Success(TValue value) =>
            new(value);

        /// <summary>
        /// Creates a new <see cref="Result{TValue}"/> with a failed result and an error.
        /// </summary>
        /// <param name="error">The error.</param>
        public static new Result<TValue> Failure(Error error) =>
            new(error);
    }
}