import React from "react";

const Button = ({
  children,
  onClick,
  type = "button",
  variant = "primary",
  fullWidth = false,
  disabled = false,
  isLoading = false,
}) => {
  const baseStyle =
    "flex justify-center items-center py-3 px-4 rounded-xl font-semibold transition-all duration-200 text-sm";

  const variants = {
    // استخدام ألوان Tailwind المخصصة التي عرفناها
    primary: "bg-primary hover:bg-primary-dark text-white shadow-md",
    outline:
      "border-2 border-gray-200 bg-transparent text-text-primary hover:border-primary hover:text-primary",
    text: "bg-transparent text-primary hover:underline",
  };

  const widthStyle = fullWidth ? "w-full" : "w-auto";
  const stateStyle =
    disabled || isLoading
      ? "opacity-60 cursor-not-allowed"
      : "cursor-pointer active:scale-95";

  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled || isLoading}
      className={`${baseStyle} ${variants[variant]} ${widthStyle} ${stateStyle}`}
    >
      {isLoading ? (
        <svg
          className="animate-spin h-5 w-5 mr-3 text-current"
          viewBox="0 0 24 24"
        >
          <circle
            className="opacity-25"
            cx="12"
            cy="12"
            r="10"
            stroke="currentColor"
            strokeWidth="4"
            fill="none"
          />
          <path
            className="opacity-75"
            fill="currentColor"
            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
          />
        </svg>
      ) : null}
      {isLoading ? "جاري المعالجة..." : children}
    </button>
  );
};

export default Button;
