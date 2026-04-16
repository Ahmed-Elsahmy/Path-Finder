import React from "react";
import { Link } from "react-router-dom";
import { useSignUp } from "../../hooks/useSignUp";
import Button from "../../../../core/ui_components/Button";
import Input from "../../../../core/ui_components/Input";

const SignUpScreen = () => {
  const { formData, isLoading, error, handleChange, handleSignUp } =
    useSignUp();

  return (
    <div className="flex flex-col items-center justify-center min-h-screen p-4 bg-gray-50">
      <div className="w-full max-w-md bg-white rounded-2xl p-8 shadow-sm">
        <div className="text-center mb-8">
          <h1 className="text-2xl font-bold mb-2">Create Account</h1>
          <p className="text-sm text-gray-500">
            Create an account to begin your journey in
            <br />
            developing your career path
          </p>
        </div>

        {error && (
          <div className="mb-4 text-sm text-red-500 text-center bg-red-50 p-2 rounded-lg">
            {error}
          </div>
        )}

        <form onSubmit={handleSignUp} className="space-y-4">
          <div className="flex gap-4">
            <Input
              name="firstName"
              placeholder="First Name"
              value={formData.firstName}
              onChange={handleChange}
            />
            <Input
              name="lastName"
              placeholder="Last Name"
              value={formData.lastName}
              onChange={handleChange}
            />
          </div>

          <Input
            name="userName"
            placeholder="User Name"
            value={formData.userName}
            onChange={handleChange}
          />
          <Input
            name="phoneNumber"
            type="tel"
            placeholder="Phone Number"
            value={formData.phoneNumber}
            onChange={handleChange}
          />
          <Input
            name="email"
            type="email"
            placeholder="Email"
            value={formData.email}
            onChange={handleChange}
          />
          <Input
            name="password"
            type="password"
            placeholder="Password"
            value={formData.password}
            onChange={handleChange}
          />
          <Input
            name="confirmPassword"
            type="password"
            placeholder="Confirm Password"
            value={formData.confirmPassword}
            onChange={handleChange}
          />

          <div className="pt-4">
            <Button type="submit" isLoading={isLoading} fullWidth>
              Create Account
            </Button>
          </div>
        </form>

        <div className="mt-6 text-center text-xs text-gray-600 font-medium">
          Already have account?{" "}
          <Link to="/login" className="text-[#5b7cfa] hover:underline">
            Login
          </Link>
        </div>
      </div>
    </div>
  );
};

export default SignUpScreen;
