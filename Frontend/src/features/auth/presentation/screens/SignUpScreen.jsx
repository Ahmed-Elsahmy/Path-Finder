import React from "react";
import { Link } from "react-router-dom";
import { useSignUp } from "../../hooks/useSignUp";
import Button from "../../../../core/ui_components/Button";
import Input from "../../../../core/ui_components/Input";

const SignUpScreen = () => {
  const { formData, isLoading, error, handleChange, handleSignUp } = useSignUp();

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50 p-4 py-12 relative overflow-hidden">
      {/* Background Decor */}
      <div className="absolute top-0 right-0 w-[500px] h-[500px] bg-gradient-to-bl from-blue-50/80 to-transparent rounded-full blur-3xl pointer-events-none opacity-50" />
      <div className="absolute bottom-0 left-0 w-[500px] h-[500px] bg-gradient-to-tr from-blue-50/50 to-transparent rounded-full blur-3xl pointer-events-none opacity-50" />

      <div className="w-full max-w-lg animate-fade-up relative z-10">
        {/* Logo */}
        <div className="flex items-center justify-center gap-2.5 mb-8">
          <div className="w-10 h-10 rounded-2xl bg-gradient-to-br from-blue-600 to-blue-700 flex items-center justify-center shadow-lg shadow-blue-500/30">
            <svg className="w-5 h-5 text-white" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M9 6.75V15m6-6v8.25m.503 3.498l4.875-2.437c.381-.19.622-.58.622-1.006V4.82c0-.836-.88-1.38-1.628-1.006l-3.869 1.934c-.317.159-.69.159-1.006 0L9.503 3.252a1.125 1.125 0 00-1.006 0L3.622 5.689C3.24 5.88 3 6.27 3 6.695V19.18c0 .836.88 1.38 1.628 1.006l3.869-1.934c.317-.159.69-.159 1.006 0l4.994 2.497c.317.158.69.158 1.006 0z"/>
            </svg>
          </div>
          <span className="font-black text-slate-900 text-xl">Path<span className="text-blue-600">Finder</span></span>
        </div>

        <div className="bg-white rounded-3xl border border-slate-200 shadow-sm p-8 sm:p-10">
          <div className="mb-8 text-center">
            <h1 className="text-2xl sm:text-3xl font-extrabold text-slate-900">Create your account</h1>
            <p className="text-slate-500 text-sm mt-2 font-medium">Start your personalized career journey today</p>
          </div>

          {error && (
            <div className="mb-6 flex items-start gap-3 p-4 bg-red-50 border border-red-100 rounded-2xl">
              <svg className="w-5 h-5 text-red-500 shrink-0 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd"/>
              </svg>
              <p className="text-sm text-red-600 font-medium">{error}</p>
            </div>
          )}

          <form onSubmit={handleSignUp} className="space-y-5">
            <div className="grid grid-cols-2 gap-4">
              <Input
                label="First Name"
                name="firstName"
                placeholder="John"
                value={formData.firstName}
                onChange={handleChange}
                required
              />
              <Input
                label="Last Name"
                name="lastName"
                placeholder="Doe"
                value={formData.lastName}
                onChange={handleChange}
                required
              />
            </div>

            <Input
              label="Username"
              name="userName"
              placeholder="johndoe"
              value={formData.userName}
              onChange={handleChange}
              required
            />

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <Input
                label="Phone Number"
                name="phoneNumber"
                type="tel"
                placeholder="+20 100 000 0000"
                value={formData.phoneNumber}
                onChange={handleChange}
              />
              <Input
                label="Email Address"
                name="email"
                type="email"
                placeholder="you@example.com"
                value={formData.email}
                onChange={handleChange}
                required
              />
            </div>

            <Input
              label="Password"
              name="password"
              type="password"
              placeholder="Min. 8 characters"
              value={formData.password}
              onChange={handleChange}
              hint="Must contain uppercase, number and special character"
              required
            />

            <Input
              label="Confirm Password"
              name="confirmPassword"
              type="password"
              placeholder="Repeat your password"
              value={formData.confirmPassword}
              onChange={handleChange}
              required
            />

            <div className="pt-4">
              <Button type="submit" isLoading={isLoading} fullWidth size="lg" className="rounded-xl shadow-md shadow-blue-600/20 active:scale-95 transition-all font-bold">
                Create Account
              </Button>
            </div>
          </form>

          <p className="mt-6 text-center text-sm font-medium text-slate-500">
            Already have an account?{" "}
            <Link to="/login" className="text-blue-600 font-bold hover:text-blue-700 transition-colors">
              Sign in
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default SignUpScreen;
