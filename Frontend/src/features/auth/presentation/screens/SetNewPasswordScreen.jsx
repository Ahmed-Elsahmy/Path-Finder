import React from "react";
import { useResetPassword } from "../../hooks/useResetPassword";
import Button from "../../../../core/ui_components/Button";
import Input from "../../../../core/ui_components/Input";

const SetNewPasswordScreen = () => {
  const { formData, isLoading, error, handleChange, handleReset } =
    useResetPassword();

  return (
    <div className="flex flex-col items-center justify-center min-h-screen p-4 bg-gray-50">
      <div className="w-full max-w-md bg-white rounded-2xl p-8 shadow-sm text-center">
        <h1 className="text-2xl font-bold mb-2">Set new Password</h1>
        <p className="text-sm text-gray-500 mb-8">
          Create a strong password to protect your
          <br />
          account. Minimum 8 characters required.
        </p>

        {error && (
          <div className="mb-4 text-sm text-red-500 bg-red-50 p-2 rounded-lg">
            {error}
          </div>
        )}

        <form onSubmit={handleReset} className="space-y-4 text-left">
          <Input
            name="newPassword"
            type="password"
            placeholder="New Password"
            value={formData.newPassword}
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
              Save Password
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default SetNewPasswordScreen;
