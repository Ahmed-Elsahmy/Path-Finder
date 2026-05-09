import React from "react";

const variants = {
  primary:
    "bg-blue-600 hover:bg-blue-700 active:bg-blue-800 text-white shadow-sm hover:shadow-md",
  secondary:
    "bg-slate-100 hover:bg-slate-200 active:bg-slate-300 text-slate-700",
  outline:
    "border border-slate-200 hover:border-slate-300 bg-white hover:bg-slate-50 text-slate-700",
  ghost:
    "bg-transparent hover:bg-slate-100 text-slate-600",
  danger:
    "bg-red-500 hover:bg-red-600 active:bg-red-700 text-white shadow-sm",
  teal:
    "bg-teal-600 hover:bg-teal-700 active:bg-teal-800 text-white shadow-sm",
};

const sizes = {
  sm:  "px-3.5 py-2 text-xs rounded-lg gap-1.5",
  md:  "px-5 py-2.5 text-sm rounded-lg gap-2",
  lg:  "px-6 py-3 text-sm rounded-xl gap-2",
  xl:  "px-8 py-3.5 text-base rounded-xl gap-2.5",
};

const Button = ({
  children,
  onClick,
  type = "button",
  variant = "primary",
  size = "md",
  fullWidth = false,
  disabled = false,
  isLoading = false,
  leftIcon,
  rightIcon,
  className = "",
}) => {
  const base =
    "inline-flex items-center justify-center font-semibold transition-all duration-150 select-none focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-500 focus-visible:ring-offset-2";
  const state =
    disabled || isLoading
      ? "opacity-50 cursor-not-allowed pointer-events-none"
      : "cursor-pointer active:scale-[.98]";

  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled || isLoading}
      className={`${base} ${variants[variant] ?? variants.primary} ${sizes[size] ?? sizes.md} ${fullWidth ? "w-full" : ""} ${state} ${className}`}
    >
      {isLoading ? (
        <svg
          className="animate-spin h-4 w-4 shrink-0"
          viewBox="0 0 24 24"
          fill="none"
        >
          <circle
            className="opacity-25"
            cx="12" cy="12" r="10"
            stroke="currentColor" strokeWidth="4"
          />
          <path
            className="opacity-75"
            fill="currentColor"
            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
          />
        </svg>
      ) : leftIcon ? (
        <span className="shrink-0">{leftIcon}</span>
      ) : null}

      <span>{isLoading ? "Processing…" : children}</span>

      {!isLoading && rightIcon && (
        <span className="shrink-0">{rightIcon}</span>
      )}
    </button>
  );
};

export default Button;
