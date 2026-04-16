/** @type {import('tailwindcss').Config} */
export default {
  // هذه الأسطر هي التي تخبر Tailwind أين يبحث عن كلاسات الألوان
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: "#5b7cfa",
          light: "#8ba4f9",
          dark: "#4a6be0",
          50: "#f0f4ff",
        },
        background: {
          DEFAULT: "#f9fafb",
          paper: "#ffffff",
        },
        text: {
          primary: "#1f2937",
          secondary: "#6b7280",
          hint: "#9ca3af",
        },
      },
    },
  },
  plugins: [],
};
