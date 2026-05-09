import React, { useState } from "react";

const EyeIcon = ({ open }) => (
  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
    {open ? (
      <>
        <path strokeLinecap="round" strokeLinejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
        <path strokeLinecap="round" strokeLinejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
      </>
    ) : (
      <>
        <path strokeLinecap="round" strokeLinejoin="round" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
      </>
    )}
  </svg>
);

const Input = ({
  label,
  name,
  type = "text",
  placeholder,
  value,
  onChange,
  error,
  hint,
  leftIcon,
  rightElement,
  required = false,
  disabled = false,
  className = "",
  inputClassName = "",
}) => {
  const [showPassword, setShowPassword] = useState(false);
  const isPassword = type === "password";
  const inputType = isPassword && showPassword ? "text" : type;

  const borderClass = error
    ? "border-red-400 focus-within:border-red-500 focus-within:ring-red-100"
    : "border-slate-200 focus-within:border-blue-500 focus-within:ring-blue-50";

  return (
    <div className={`flex flex-col gap-1.5 w-full ${className}`}>
      {label && (
        <label
          htmlFor={name}
          className="text-sm font-medium text-slate-700 flex items-center gap-1"
        >
          {label}
          {required && <span className="text-red-500 text-xs">*</span>}
        </label>
      )}

      <div
        className={`relative flex items-center bg-white border rounded-lg overflow-hidden transition-all duration-150 focus-within:ring-3 ${borderClass} ${disabled ? "opacity-60 bg-slate-50" : ""}`}
      >
        {leftIcon && (
          <span className="pl-3.5 text-slate-400 flex items-center shrink-0">
            {leftIcon}
          </span>
        )}

        <input
          id={name}
          name={name}
          type={inputType}
          placeholder={placeholder}
          value={value}
          onChange={onChange}
          disabled={disabled}
          autoComplete={isPassword ? "current-password" : undefined}
          className={`w-full py-2.5 px-3.5 text-sm text-slate-800 bg-transparent outline-none placeholder-slate-400 disabled:cursor-not-allowed ${inputClassName}`}
        />

        {isPassword && (
          <button
            type="button"
            tabIndex={-1}
            onClick={() => setShowPassword((v) => !v)}
            className="pr-3.5 text-slate-400 hover:text-blue-500 transition-colors focus:outline-none shrink-0"
            aria-label={showPassword ? "Hide password" : "Show password"}
          >
            <EyeIcon open={showPassword} />
          </button>
        )}

        {!isPassword && rightElement && (
          <div className="pr-3.5 shrink-0">{rightElement}</div>
        )}
      </div>

      {error && (
        <p className="text-xs text-red-500 font-medium flex items-center gap-1">
          <svg className="w-3 h-3 shrink-0" fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
          </svg>
          {error}
        </p>
      )}
      {!error && hint && (
        <p className="text-xs text-slate-400">{hint}</p>
      )}
    </div>
  );
};

export default Input;
