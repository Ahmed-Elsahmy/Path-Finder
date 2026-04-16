import React, { useState } from "react";

const Input = ({
  label,
  name,
  type = "text",
  placeholder,
  value,
  onChange,
  error,
  leftIcon,
}) => {
  const [showPassword, setShowPassword] = useState(false);
  const isPassword = type === "password";
  const inputType = isPassword && showPassword ? "text" : type;

  return (
    <div className="flex flex-col mb-4 w-full">
      {/* عنوان الحقل */}
      {label && (
        <label className="mb-1 text-sm font-medium text-text-primary pl-1">
          {label}
        </label>
      )}

      {/* حاوية الإدخال */}
      <div
        className={`
        relative flex items-center bg-white border rounded-xl overflow-hidden transition-all duration-200
        ${
          error
            ? "border-red-500 ring-1 ring-red-500"
            : "border-gray-300 focus-within:border-primary focus-within:ring-1 focus-within:ring-primary"
        }
      `}
      >
        {/* الأيقونة الجانبية إن وجدت */}
        {leftIcon && (
          <span className="pl-4 text-text-hint flex items-center justify-center">
            {leftIcon}
          </span>
        )}

        <input
          name={name}
          type={inputType}
          placeholder={placeholder}
          value={value}
          onChange={onChange}
          className="w-full py-3 px-4 text-sm text-text-primary bg-transparent outline-none placeholder-text-hint"
        />

        {/* زر إظهار/إخفاء كلمة المرور */}
        {isPassword && (
          <button
            type="button"
            onClick={() => setShowPassword(!showPassword)}
            className="pr-4 text-text-hint hover:text-primary focus:outline-none text-xs font-semibold transition-colors"
          >
            {showPassword ? "إخفاء" : "إظهار"}
          </button>
        )}
      </div>

      {/* رسالة الخطأ */}
      {error && <span className="mt-1 text-xs text-red-500 pl-1">{error}</span>}
    </div>
  );
};

export default Input;
