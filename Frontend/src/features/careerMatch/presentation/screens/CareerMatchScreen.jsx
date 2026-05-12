import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useCareerMatch } from "../../hooks/useCareerMatch";

const CareerMatchScreen = () => {
  const {
    assessment,
    answers,
    result,
    questions,
    totalQuestions,
    answeredCount,
    requiredTotal,
    requiredAnsweredCount,
    isLoading,
    isSubmitting,
    enrollingId,
    error,
    notice,
    setAnswer,
    submitAssessment,
    enrollInPath,
  } = useCareerMatch();

  const [currentIndex, setCurrentIndex] = useState(0);
  const [inlineMessage, setInlineMessage] = useState("");

  useEffect(() => {
    if (!questions.length) {
      return;
    }

    const firstRequiredUnanswered = questions.findIndex((question) => {
      if (!question.isRequired) {
        return false;
      }

      const value = answers[question.questionId] || "";
      return !value.trim();
    });

    setCurrentIndex(firstRequiredUnanswered >= 0 ? firstRequiredUnanswered : 0);
  }, [assessment?.questionnaireId, questions, answers]);

  const currentQuestion = questions[currentIndex];
  const currentValue = currentQuestion
    ? answers[currentQuestion.questionId] || ""
    : "";

  const overallProgress =
    totalQuestions === 0 ? 0 : Math.round((answeredCount / totalQuestions) * 100);
  const requiredProgress =
    requiredTotal === 0
      ? 0
      : Math.round((requiredAnsweredCount / requiredTotal) * 100);

  const canMoveForward =
    !currentQuestion ||
    !currentQuestion.isRequired ||
    currentValue.trim().length > 0;

  const handleNext = () => {
    if (!canMoveForward) {
      setInlineMessage("Please select an answer to continue.");
      return;
    }

    setInlineMessage("");
    setCurrentIndex((index) => Math.min(index + 1, totalQuestions - 1));
  };

  const handlePrevious = () => {
    setInlineMessage("");
    setCurrentIndex((index) => Math.max(index - 1, 0));
  };

  const handleSubmit = async () => {
    if (requiredAnsweredCount < requiredTotal) {
      setInlineMessage("Please complete all required questions to unlock your matches.");
      return;
    }

    setInlineMessage("");
    const data = await submitAssessment();

    if (data) {
      window.setTimeout(() => {
        document
          .getElementById("career-match-results")
          ?.scrollIntoView({ behavior: "smooth", block: "start" });
      }, 100);
    }
  };

  if (isLoading) {
    return (
      <div className="flex flex-col justify-center items-center py-32 space-y-4">
        <div className="animate-spin rounded-full h-10 w-10 border-[3px] border-slate-200 border-t-blue-600"></div>
        <p className="text-slate-500 font-medium animate-pulse">Loading assessment...</p>
      </div>
    );
  }

  if (!assessment || !currentQuestion) {
    return (
      <div className="rounded-2xl border border-red-200 bg-red-50 p-6 flex flex-col items-center justify-center text-center space-y-3">
        <div className="w-12 h-12 bg-red-100 text-red-500 rounded-full flex items-center justify-center">
          <svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/></svg>
        </div>
        <p className="text-red-700 font-medium">
          {error || "The career assessment is currently unavailable."}
        </p>
      </div>
    );
  }

  return (
    <div className="space-y-8 pb-16 animate-fade-up">
      {/* Premium Hero Section */}
      <section className="relative overflow-hidden rounded-3xl bg-white border border-slate-200 shadow-sm">
        {/* Subtle Background Gradients */}
        <div className="absolute top-0 right-0 w-full h-full bg-gradient-to-br from-blue-50/50 via-white to-white pointer-events-none" />
        <div className="absolute -top-24 -right-24 w-96 h-96 bg-blue-100/50 rounded-full blur-3xl pointer-events-none" />
        <div className="absolute -bottom-24 -left-24 w-96 h-96 bg-indigo-50 rounded-full blur-3xl pointer-events-none" />

        <div className="relative z-10 grid gap-10 lg:grid-cols-2 lg:items-center p-8 sm:p-12">
          <div className="space-y-6">
            <div className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-blue-50 border border-blue-100">
              <span className="flex h-2 w-2 rounded-full bg-blue-600"></span>
              <span className="text-[11px] font-bold uppercase tracking-wider text-blue-700">
                Career Intelligence
              </span>
            </div>
            <div className="space-y-4">
              <h2 className="text-4xl font-extrabold tracking-tight text-slate-900 leading-tight">
                Discover the career path <span className="text-blue-600">built for you.</span>
              </h2>
              <p className="text-base leading-relaxed text-slate-600 max-w-lg">
                {assessment.description ||
                  "Answer a short series of questions to receive AI-driven, highly accurate career recommendations tailored to your unique strengths and goals."}
              </p>
            </div>
            <div className="flex flex-wrap gap-4 pt-2">
              <div className="flex items-center gap-2">
                <svg className="w-5 h-5 text-blue-500" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>
                <span className="text-sm font-semibold text-slate-700">Fast & Guided</span>
              </div>
              <div className="flex items-center gap-2">
                <svg className="w-5 h-5 text-blue-500" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M3 13h2.626c.825 0 1.594-.4 2.072-1.077L10.375 8A2.5 2.5 0 0112.5 6.5h7M16 3l4 4-4 4"/></svg>
                <span className="text-sm font-semibold text-slate-700">AI-Ranked Matches</span>
              </div>
              <div className="flex items-center gap-2">
                <svg className="w-5 h-5 text-blue-500" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M13 10V3L4 14h7v7l9-11h-7z"/></svg>
                <span className="text-sm font-semibold text-slate-700">Instantly Actionable</span>
              </div>
            </div>
          </div>

          {/* Progress Widget */}
          <div className="bg-white rounded-2xl border border-slate-200/60 p-6 shadow-xl shadow-blue-900/5">
            <div className="flex items-center justify-between text-sm font-semibold text-slate-600 mb-3">
              <span>Assessment Progress</span>
              <span className="text-blue-600">{overallProgress}% Complete</span>
            </div>
            <div className="h-2 w-full bg-slate-100 rounded-full overflow-hidden">
              <div
                className="h-full bg-gradient-to-r from-blue-500 to-blue-600 transition-all duration-700 ease-out rounded-full"
                style={{ width: `${overallProgress}%` }}
              />
            </div>

            <div className="mt-8 grid grid-cols-3 gap-4 divide-x divide-slate-100">
              <div className="text-center px-2">
                <p className="text-[10px] font-bold uppercase tracking-widest text-slate-400 mb-1">Answered</p>
                <p className="text-2xl font-extrabold text-slate-900">{answeredCount}</p>
              </div>
              <div className="text-center px-2">
                <p className="text-[10px] font-bold uppercase tracking-widest text-slate-400 mb-1">Required</p>
                <p className="text-2xl font-extrabold text-slate-900">{requiredProgress}%</p>
              </div>
              <div className="text-center px-2">
                <p className="text-[10px] font-bold uppercase tracking-widest text-slate-400 mb-1">Status</p>
                <p className="text-xs font-bold text-slate-700 mt-2 line-clamp-2">
                  {result?.recommendations?.length
                    ? "Ranked"
                    : assessment.hasSavedResponses
                      ? "In Progress"
                      : "Ready"}
                </p>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Alerts */}
      {(error || notice || inlineMessage) && (
        <div className="space-y-3">
          {error && (
            <div className="rounded-xl border border-red-200 bg-red-50/50 px-4 py-3 text-sm font-medium text-red-600 flex items-center gap-3">
              <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/></svg>
              {error}
            </div>
          )}
          {notice && (
            <div className="rounded-xl border border-blue-200 bg-blue-50/50 px-4 py-3 text-sm font-medium text-blue-700 flex items-center gap-3">
              <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>
              {notice}
            </div>
          )}
          {inlineMessage && (
            <div className="rounded-xl border border-amber-200 bg-amber-50/50 px-4 py-3 text-sm font-medium text-amber-700 flex items-center gap-3">
              <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/></svg>
              {inlineMessage}
            </div>
          )}
        </div>
      )}

      {/* Main Assessment Layout */}
      <div className="grid gap-8 lg:grid-cols-[1fr_320px] xl:grid-cols-[1.3fr_380px]">
        {/* Left Column: Active Question */}
        <div className="flex flex-col h-full">
          <div className="rounded-3xl border border-slate-200 bg-white shadow-sm flex flex-col flex-1">
            <div className="p-8 sm:p-10 flex-1">
              <div className="flex flex-col gap-4 border-b border-slate-100 pb-6 mb-8 sm:flex-row sm:items-end sm:justify-between">
                <div>
                  <p className="text-[11px] font-bold uppercase tracking-widest text-blue-600 mb-2">
                    Question {currentIndex + 1} of {totalQuestions}
                  </p>
                  <h3 className="text-2xl sm:text-3xl font-bold tracking-tight text-slate-900 leading-snug">
                    {currentQuestion.questionText}
                  </h3>
                </div>

                <div className="inline-flex shrink-0 items-center justify-center rounded-xl bg-slate-50 border border-slate-100 px-3 py-1.5 text-xs font-semibold text-slate-500">
                  {currentQuestion.isRequired ? (
                    <><span className="w-1.5 h-1.5 rounded-full bg-blue-500 mr-2"></span>Required</>
                  ) : (
                    <><span className="w-1.5 h-1.5 rounded-full bg-slate-300 mr-2"></span>Optional</>
                  )}
                </div>
              </div>

              {/* Question Input */}
              <div className="animate-fade-in">
                {currentQuestion.questionType === "text" ? (
                  <div className="space-y-4">
                    <label className="block text-sm font-medium text-slate-600">
                      Share your thoughts in detail.
                    </label>
                    <textarea
                      value={currentValue}
                      onChange={(event) =>
                        setAnswer(currentQuestion.questionId, event.target.value)
                      }
                      rows={6}
                      placeholder="Type your answer here..."
                      className="w-full rounded-2xl border border-slate-200 bg-slate-50/50 p-5 text-sm leading-relaxed text-slate-800 outline-none transition-all placeholder:text-slate-400 focus:border-blue-500 focus:bg-white focus:ring-4 focus:ring-blue-50 resize-none"
                    />
                  </div>
                ) : (
                  <div className="grid gap-4 sm:grid-cols-2">
                    {(currentQuestion.options || []).map((option) => {
                      const isSelected = currentValue === option;

                      return (
                        <button
                          key={option}
                          type="button"
                          onClick={() => {
                            setInlineMessage("");
                            setAnswer(currentQuestion.questionId, option);
                          }}
                          className={`group relative overflow-hidden rounded-2xl border p-5 text-left transition-all duration-200 ${
                            isSelected
                              ? "border-blue-600 bg-blue-50/30 shadow-md shadow-blue-900/5 ring-1 ring-blue-600"
                              : "border-slate-200 bg-white hover:border-blue-300 hover:shadow-sm"
                          }`}
                        >
                          {isSelected && (
                            <div className="absolute top-0 right-0 w-16 h-16 bg-blue-100/50 rounded-bl-full -z-10" />
                          )}
                          <div className="flex items-start gap-4">
                            <div
                              className={`mt-0.5 flex h-6 w-6 shrink-0 items-center justify-center rounded-full border transition-all ${
                                isSelected
                                  ? "border-blue-600 bg-blue-600 text-white"
                                  : "border-slate-300 bg-slate-50 text-slate-400 group-hover:border-blue-300 group-hover:bg-blue-50"
                              }`}
                            >
                              {isSelected ? (
                                <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M4.5 12.8l3.6 3.6 11.4-11.4"/></svg>
                              ) : (
                                <span className="text-[10px] font-bold">{currentQuestion.orderNumber}</span>
                              )}
                            </div>
                            <div>
                              <p className={`text-sm font-semibold leading-relaxed ${isSelected ? "text-blue-900" : "text-slate-700"}`}>
                                {option}
                              </p>
                            </div>
                          </div>
                        </button>
                      );
                    })}
                  </div>
                )}
              </div>
            </div>

            {/* Navigation Footer */}
            <div className="bg-slate-50 border-t border-slate-100 p-6 sm:px-10 rounded-b-3xl flex flex-col-reverse gap-4 sm:flex-row sm:items-center sm:justify-between">
              <p className="text-xs font-semibold text-slate-500 text-center sm:text-left">
                {requiredAnsweredCount} of {requiredTotal} required answered
              </p>
              
              <div className="flex gap-3 w-full sm:w-auto">
                <button
                  type="button"
                  onClick={handlePrevious}
                  disabled={currentIndex === 0}
                  className="flex-1 sm:flex-none rounded-xl border border-slate-200 bg-white px-6 py-2.5 text-sm font-semibold text-slate-600 transition-all hover:bg-slate-50 hover:text-slate-900 disabled:cursor-not-allowed disabled:opacity-50 shadow-sm"
                >
                  Previous
                </button>
                {currentIndex < totalQuestions - 1 ? (
                  <button
                    type="button"
                    onClick={handleNext}
                    className="flex-1 sm:flex-none rounded-xl bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white shadow-sm shadow-blue-600/20 transition-all hover:-translate-y-0.5 hover:bg-blue-700 hover:shadow-md"
                  >
                    Continue
                  </button>
                ) : (
                  <button
                    type="button"
                    onClick={handleSubmit}
                    disabled={isSubmitting}
                    className="flex-1 sm:flex-none inline-flex items-center justify-center gap-2 rounded-xl bg-slate-900 px-6 py-2.5 text-sm font-bold text-white shadow-md transition-all hover:-translate-y-0.5 hover:bg-slate-800 hover:shadow-lg disabled:cursor-not-allowed disabled:opacity-70"
                  >
                    {isSubmitting ? (
                      <><div className="w-4 h-4 border-2 border-white/20 border-t-white rounded-full animate-spin"></div> Analyzing...</>
                    ) : (
                      "Generate Matches"
                    )}
                  </button>
                )}
              </div>
            </div>
          </div>
        </div>

        {/* Right Column: Sidebar */}
        <aside className="space-y-6">
          <div className="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
            <h3 className="text-sm font-bold text-slate-900 mb-4 uppercase tracking-wider">
              Assessment Map
            </h3>
            
            <div className="space-y-2">
              {questions.map((question, index) => {
                const value = answers[question.questionId] || "";
                const isAnswered = value.trim().length > 0;
                const isActive = index === currentIndex;

                return (
                  <button
                    key={question.questionId}
                    type="button"
                    onClick={() => {
                      setInlineMessage("");
                      setCurrentIndex(index);
                    }}
                    className={`flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-left transition-all ${
                      isActive
                        ? "bg-blue-50 ring-1 ring-blue-200"
                        : "hover:bg-slate-50"
                    }`}
                  >
                    <div
                      className={`flex h-5 w-5 shrink-0 items-center justify-center rounded-full text-[9px] font-bold ${
                        isActive
                          ? "bg-blue-600 text-white"
                          : isAnswered
                            ? "bg-slate-800 text-white"
                            : "bg-slate-100 text-slate-400 border border-slate-200"
                      }`}
                    >
                      {isAnswered && !isActive ? (
                        <svg className="w-3 h-3" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M4.5 12.8l3.6 3.6 11.4-11.4"/></svg>
                      ) : (
                        index + 1
                      )}
                    </div>
                    <p className={`truncate text-xs font-medium ${isActive ? "text-blue-900" : isAnswered ? "text-slate-700" : "text-slate-500"}`}>
                      {question.questionText}
                    </p>
                  </button>
                );
              })}
            </div>
          </div>

          {!result ? (
            <div className="rounded-3xl border border-slate-200 bg-slate-50 p-6 shadow-sm">
              <h3 className="text-sm font-bold text-slate-900 mb-4 uppercase tracking-wider">
                How It Works
              </h3>
              <ul className="space-y-4 relative before:absolute before:inset-y-2 before:left-[11px] before:w-0.5 before:bg-slate-200">
                <li className="relative flex gap-4">
                  <div className="w-6 h-6 rounded-full bg-white border-2 border-slate-200 flex items-center justify-center shrink-0 z-10 text-[10px] font-bold text-slate-500">1</div>
                  <div>
                    <p className="text-sm font-semibold text-slate-800">Answer Questions</p>
                    <p className="text-xs text-slate-500 mt-1">Focus on your instincts and what genuinely interests you.</p>
                  </div>
                </li>
                <li className="relative flex gap-4">
                  <div className="w-6 h-6 rounded-full bg-white border-2 border-slate-200 flex items-center justify-center shrink-0 z-10 text-[10px] font-bold text-slate-500">2</div>
                  <div>
                    <p className="text-sm font-semibold text-slate-800">AI Analysis</p>
                    <p className="text-xs text-slate-500 mt-1">Our engine matches your profile against real-world career data.</p>
                  </div>
                </li>
                <li className="relative flex gap-4">
                  <div className="w-6 h-6 rounded-full bg-white border-2 border-slate-200 flex items-center justify-center shrink-0 z-10 text-[10px] font-bold text-slate-500">3</div>
                  <div>
                    <p className="text-sm font-semibold text-slate-800">Actionable Paths</p>
                    <p className="text-xs text-slate-500 mt-1">Review ranked results and start learning immediately.</p>
                  </div>
                </li>
              </ul>
            </div>
          ) : (
            <div className="rounded-3xl border border-blue-200 bg-blue-50 p-6 shadow-sm relative overflow-hidden">
              <div className="absolute top-0 right-0 w-32 h-32 bg-blue-200/50 rounded-bl-full -z-10 blur-xl"></div>
              <h3 className="text-sm font-bold text-blue-900 mb-4 uppercase tracking-wider flex items-center gap-2">
                <svg className="w-4 h-4 text-blue-600" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M13 10V3L4 14h7v7l9-11h-7z"/></svg>
                AI Insights
              </h3>
              <p className="text-sm text-blue-800 leading-relaxed mb-4">
                {result.profileSummary}
              </p>
              <div className="flex flex-wrap gap-1.5">
                {(result.topTraits || []).map(trait => (
                  <span key={trait} className="px-2.5 py-1 bg-white text-blue-700 text-[10px] font-bold uppercase tracking-widest rounded-md border border-blue-100 shadow-sm">
                    {trait}
                  </span>
                ))}
              </div>
            </div>
          )}
        </aside>
      </div>

      {/* Results Section */}
      {result?.recommendations?.length > 0 && (
        <section id="career-match-results" className="pt-10 mt-10 border-t border-slate-200 space-y-8 animate-fade-up">
          <div className="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
            <div>
              <p className="text-[11px] font-bold uppercase tracking-widest text-blue-600 mb-1">
                Your Matches
              </p>
              <h3 className="text-3xl font-extrabold tracking-tight text-slate-900">
                Highly Recommended Paths
              </h3>
            </div>
          </div>

          <div className="grid gap-6 lg:grid-cols-2">
            {result.recommendations.map((recommendation, index) => (
              <div
                key={recommendation.careerPathId}
                className="group flex flex-col rounded-3xl border border-slate-200 bg-white overflow-hidden shadow-sm hover:shadow-xl hover:border-blue-200 transition-all duration-300"
              >
                <div className="p-6 sm:p-8 flex-1 flex flex-col">
                  <div className="flex items-start justify-between mb-6">
                    <div className="inline-flex items-center justify-center w-12 h-12 rounded-2xl bg-slate-50 text-xl font-black text-slate-800 group-hover:bg-blue-600 group-hover:text-white transition-colors shadow-sm">
                      #{index + 1}
                    </div>
                    <div className="text-right">
                      <p className="text-[10px] font-bold uppercase tracking-widest text-slate-400 mb-1">Match Score</p>
                      <div className="flex items-baseline justify-end gap-1 text-blue-600">
                        <span className="text-3xl font-black tracking-tight">{recommendation.suitabilityScore}</span>
                        <span className="text-lg font-bold">%</span>
                      </div>
                    </div>
                  </div>

                  <div className="mb-4">
                    {recommendation.categoryName && (
                      <span className="text-[11px] font-bold text-slate-400 uppercase tracking-widest mb-1 block">
                        {recommendation.categoryName} {recommendation.subCategoryName ? ` / ${recommendation.subCategoryName}` : ''}
                      </span>
                    )}
                    <h4 className="text-2xl font-bold text-slate-900 leading-tight">
                      {recommendation.careerPathName}
                    </h4>
                  </div>

                  <p className="text-sm text-slate-600 leading-relaxed mb-6 flex-1">
                    {recommendation.whyItFits}
                  </p>

                  <div className="space-y-4 border-t border-slate-100 pt-5">
                    {recommendation.strengthSignals?.length > 0 && (
                      <div>
                        <p className="text-[10px] font-bold uppercase tracking-widest text-slate-400 mb-2">Key Strengths Match</p>
                        <div className="flex flex-wrap gap-1.5">
                          {recommendation.strengthSignals.map(sig => (
                            <span key={sig} className="px-2 py-1 bg-slate-50 text-slate-600 text-xs font-semibold rounded-md border border-slate-100">
                              {sig}
                            </span>
                          ))}
                        </div>
                      </div>
                    )}
                    
                    <div className="flex items-center gap-4 text-xs font-semibold text-slate-500">
                      {recommendation.difficultyLevel && (
                        <span className="flex items-center gap-1.5">
                          <svg className="w-4 h-4 text-slate-400" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M3 13.125C3 12.504 3.504 12 4.125 12h2.25c.621 0 1.125.504 1.125 1.125v6.75C7.5 20.496 6.996 21 6.375 21h-2.25A1.125 1.125 0 013 19.875v-6.75zM9.75 8.625c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125v11.25c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 01-1.125-1.125V8.625zM16.5 4.125c0-.621.504-1.125 1.125-1.125h2.25C20.496 3 21 3.504 21 4.125v15.75c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 01-1.125-1.125V4.125z"/></svg>
                          {recommendation.difficultyLevel}
                        </span>
                      )}
                      {recommendation.durationInMonths && (
                        <span className="flex items-center gap-1.5">
                          <svg className="w-4 h-4 text-slate-400" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>
                          {recommendation.durationInMonths} mos
                        </span>
                      )}
                      <span className="flex items-center gap-1.5">
                         <svg className="w-4 h-4 text-slate-400" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253"/></svg>
                        {recommendation.totalCourses} courses
                      </span>
                    </div>
                  </div>
                </div>

                <div className="bg-slate-50 p-4 sm:px-8 sm:py-5 border-t border-slate-100 flex flex-col gap-3 sm:flex-row sm:items-center">
                  <button
                    type="button"
                    onClick={() => enrollInPath(recommendation.careerPathId)}
                    disabled={recommendation.isAlreadyEnrolled || enrollingId === recommendation.careerPathId}
                    className={`flex-1 rounded-xl px-4 py-2.5 text-sm font-bold transition-all shadow-sm ${
                      recommendation.isAlreadyEnrolled
                        ? "bg-slate-200 text-slate-500 cursor-not-allowed"
                        : "bg-blue-600 text-white hover:bg-blue-700 hover:shadow-md hover:-translate-y-0.5"
                    }`}
                  >
                    {recommendation.isAlreadyEnrolled
                      ? "Enrolled"
                      : enrollingId === recommendation.careerPathId
                        ? "Starting..."
                        : "Start Path"}
                  </button>
                  <Link
                    to="/ai-assistant"
                    className="flex-1 text-center rounded-xl px-4 py-2.5 text-sm font-bold text-slate-600 bg-white border border-slate-200 transition-all hover:bg-slate-50 hover:text-slate-900"
                  >
                    Ask AI
                  </Link>
                </div>
              </div>
            ))}
          </div>
        </section>
      )}
    </div>
  );
};

export default CareerMatchScreen;
