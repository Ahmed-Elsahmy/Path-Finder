import React, { useState, useEffect } from "react";
import { useProfile } from "../../hooks/useProfile";

const EditProfileModal = ({ isOpen, onClose, currentUser }) => {
  const { updateProfile, isUpdating } = useProfile();

  // 1. إضافة حقل email للحالة (State)
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    userName: "",
    phoneNumber: "",
    bio: "",
    location: "",
    email: "", // 👈 إضافة حقل الإيميل هنا
  });
  const [selectedImage, setSelectedImage] = useState(null);

  // 2. تعبئة البيانات عند فتح المودال
  useEffect(() => {
    if (currentUser) {
      setFormData({
        firstName: currentUser.firstName || currentUser.FirstName || "",
        lastName: currentUser.lastName || currentUser.LastName || "",
        userName: currentUser.userName || currentUser.UserName || "",
        phoneNumber: currentUser.phoneNumber || currentUser.PhoneNumber || "",
        bio: currentUser.bio || currentUser.Bio || "",
        location: currentUser.location || currentUser.Location || "",
        // 💡 سحب الإيميل من الباك إند أو من التخزين المحلي
        email:
          currentUser.email ||
          currentUser.Email ||
          localStorage.getItem("userEmail") ||
          "",
      });
    }
  }, [currentUser]);

  const onFormChange = (e) =>
    setFormData({ ...formData, [e.target.name]: e.target.value });

  const onFileChange = (e) => setSelectedImage(e.target.files[0]);

  const onSubmit = async (e) => {
    e.preventDefault();

    // 💡 حفظ الإيميل في التخزين المحلي ليظهر في كامل السيستم فوراً
    if (formData.email) {
      localStorage.setItem("userEmail", formData.email);
    }

    const success = await updateProfile(formData, selectedImage);
    if (success) {
      onClose();
    }
  };

  if (!isOpen) return null;

  // معالجة مسار الصورة الحالية للمعاينة
  const rawPic =
    currentUser?.profilePictureUrl || currentUser?.ProfilePictureUrl;
  const currentProfilePic = rawPic
    ? rawPic.startsWith("http")
      ? rawPic
      : `https://pathfinder.tryasp.net${rawPic}`
    : null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm p-4 animate-fade-in">
      <div className="bg-white w-full max-w-2xl rounded-3xl shadow-xl overflow-hidden flex flex-col max-h-[90vh]">
        {/* Header */}
        <div className="p-6 border-b border-gray-100 flex justify-between items-center bg-slate-50">
          <h2 className="text-xl font-bold text-gray-900">Edit Profile</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-red-500 font-bold transition-colors"
          >
            ✖
          </button>
        </div>

        <div className="p-6 overflow-y-auto flex-1">
          <form
            id="edit-profile-form"
            onSubmit={onSubmit}
            className="space-y-5"
          >
            {/* Profile Picture Section */}
            <div className="flex items-center gap-4 p-4 bg-slate-50 rounded-2xl border border-dashed border-gray-200">
              <div className="w-16 h-16 bg-primary/10 text-primary rounded-xl flex items-center justify-center text-xl font-black overflow-hidden border-2 border-white shadow-sm">
                {selectedImage ? (
                  <img
                    src={URL.createObjectURL(selectedImage)}
                    alt="Preview"
                    className="w-full h-full object-cover"
                  />
                ) : currentProfilePic ? (
                  <img
                    src={currentProfilePic}
                    alt="Current"
                    className="w-full h-full object-cover"
                  />
                ) : (
                  <>{formData.firstName?.[0] || "👤"}</>
                )}
              </div>
              <div>
                <label className="block text-sm font-bold text-gray-700 mb-2">
                  Profile Picture
                </label>
                <label
                  htmlFor="profile-upload"
                  className="bg-primary/10 text-primary hover:bg-primary hover:text-white px-4 py-2 rounded-xl font-bold transition-all cursor-pointer text-sm inline-block"
                >
                  {selectedImage || currentProfilePic
                    ? "Change Photo"
                    : "Upload Photo"}
                </label>
                <input
                  id="profile-upload"
                  type="file"
                  accept="image/*"
                  onChange={onFileChange}
                  className="hidden"
                />
              </div>
            </div>

            {/* Form Fields Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="text-xs text-gray-500 font-bold ml-1 mb-1 block">
                  First Name
                </label>
                <input
                  required
                  type="text"
                  name="firstName"
                  value={formData.firstName}
                  onChange={onFormChange}
                  className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none"
                />
              </div>
              <div>
                <label className="text-xs text-gray-500 font-bold ml-1 mb-1 block">
                  Last Name
                </label>
                <input
                  required
                  type="text"
                  name="lastName"
                  value={formData.lastName}
                  onChange={onFormChange}
                  className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none"
                />
              </div>
              <div>
                <label className="text-xs text-gray-500 font-bold ml-1 mb-1 block">
                  Username
                </label>
                <input
                  required
                  type="text"
                  name="userName"
                  value={formData.userName}
                  onChange={onFormChange}
                  className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none"
                />
              </div>
              <div>
                <label className="text-xs text-gray-500 font-bold ml-1 mb-1 block">
                  Phone Number
                </label>
                <input
                  type="tel"
                  name="phoneNumber"
                  value={formData.phoneNumber}
                  onChange={onFormChange}
                  className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none"
                />
              </div>

              {/* حقل الموقع */}
              <div className="md:col-span-2">
                <label className="text-xs text-gray-500 font-bold ml-1 mb-1 block">
                  Location
                </label>
                <input
                  type="text"
                  name="location"
                  value={formData.location}
                  onChange={onFormChange}
                  placeholder="e.g., Cairo, Egypt"
                  className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none"
                />
              </div>

              {/* حقل الإيميل الجديد 💡 */}
              <div className="md:col-span-2">
                <label className="text-xs text-gray-500 font-bold ml-1 mb-1 block">
                  Email Address (Personal Reference)
                </label>
                <input
                  type="email"
                  name="email"
                  value={formData.email}
                  onChange={onFormChange}
                  placeholder="your-email@example.com"
                  className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none"
                />
              </div>

              {/* حقل البايو */}
              <div className="md:col-span-2">
                <label className="text-xs text-gray-500 font-bold ml-1 mb-1 block">
                  Bio
                </label>
                <textarea
                  name="bio"
                  value={formData.bio}
                  onChange={onFormChange}
                  rows="3"
                  className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none resize-none"
                ></textarea>
              </div>
            </div>
          </form>
        </div>

        {/* Footer */}
        <div className="p-6 border-t border-gray-100 bg-slate-50 flex justify-end gap-3">
          <button
            type="button"
            onClick={onClose}
            className="px-6 py-2 text-gray-500 font-bold hover:bg-gray-200 rounded-xl transition-colors"
          >
            Cancel
          </button>
          <button
            type="submit"
            form="edit-profile-form"
            disabled={isUpdating}
            className="px-8 py-2 bg-primary text-white font-bold rounded-xl hover:bg-primary-dark disabled:opacity-50 transition-colors shadow-md"
          >
            {isUpdating ? "Saving..." : "Save Changes"}
          </button>
        </div>
      </div>
    </div>
  );
};

export default EditProfileModal;
