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
  }, [assessment?.questionnaireId]);

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
      setInlineMessage("Choose an answer before moving to the next question.");
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
      setInlineMessage("Finish the required questions to unlock your matches.");
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
      <div className="flex justify-center items-center py-24">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[#0f766e]"></div>
      </div>
    );
  }

  if (!assessment || !currentQuestion) {
    return (
      <div className="rounded-[2rem] border border-red-100 bg-red-50 px-6 py-5 text-red-600">
        {error || "The career assessment is not available right now."}
      </div>
    );
  }

  return (
    <div className="space-y-8 pb-12">
      <section className="relative overflow-hidden rounded-[2.75rem] bg-[radial-gradient(circle_at_top_left,_rgba(45,212,191,0.28),_transparent_38%),linear-gradient(135deg,#062f2d_0%,#0f766e_42%,#164e63_100%)] px-8 py-10 text-white shadow-2xl shadow-cyan-950/10 sm:px-10">
        <div className="relative z-10 grid gap-8 xl:grid-cols-[1.1fr_0.9fr] xl:items-end">
          <div className="space-y-5">
            <span className="inline-flex rounded-full border border-white/15 bg-white/10 px-4 py-2 text-[11px] font-black uppercase tracking-[0.3em] text-teal-50">
              Career Match
            </span>
            <div className="space-y-4">
              <h2 className="max-w-2xl text-4xl font-black tracking-tight sm:text-5xl">
                Find the path that fits how you think, build, and grow.
              </h2>
              <p className="max-w-2xl text-base leading-7 text-teal-50/85 sm:text-lg">
                {assessment.description ||
                  "Answer a short guided test and let Path Finder rank the strongest career paths already available in your platform database."}
              </p>
            </div>
            <div className="flex flex-wrap gap-3 pt-2">
              <div className="rounded-2xl border border-white/10 bg-white/10 px-4 py-3">
                <p className="text-[11px] font-black uppercase tracking-[0.25em] text-teal-100">
                  Guided
                </p>
                <p className="mt-1 text-sm font-semibold text-white">
                  9 focused questions
                </p>
              </div>
              <div className="rounded-2xl border border-white/10 bg-white/10 px-4 py-3">
                <p className="text-[11px] font-black uppercase tracking-[0.25em] text-teal-100">
                  Ranked
                </p>
                <p className="mt-1 text-sm font-semibold text-white">
                  Multiple career matches
                </p>
              </div>
              <div className="rounded-2xl border border-white/10 bg-white/10 px-4 py-3">
                <p className="text-[11px] font-black uppercase tracking-[0.25em] text-teal-100">
                  Actionable
                </p>
                <p className="mt-1 text-sm font-semibold text-white">
                  Start a path in one click
                </p>
              </div>
            </div>
          </div>

          <div className="rounded-[2rem] border border-white/10 bg-white/10 p-6 backdrop-blur-md">
            <div className="flex items-center justify-between text-sm font-semibold text-teal-50/90">
              <span>Assessment progress</span>
              <span>{overallProgress}% complete</span>
            </div>
            <div className="mt-4 h-3 overflow-hidden rounded-full bg-white/10">
              <div
                className="h-full rounded-full bg-gradient-to-r from-[#facc15] via-[#f59e0b] to-[#fb7185] transition-all duration-500"
                style={{ width: `${overallProgress}%` }}
              ></div>
            </div>

            <div className="mt-6 grid gap-4 sm:grid-cols-3">
              <div className="rounded-2xl bg-black/10 px-4 py-4">
                <p className="text-[11px] font-black uppercase tracking-[0.25em] text-teal-100">
                  Answered
                </p>
                <p className="mt-2 text-3xl font-black">{answeredCount}</p>
              </div>
              <div className="rounded-2xl bg-black/10 px-4 py-4">
                <p className="text-[11px] font-black uppercase tracking-[0.25em] text-teal-100">
                  Required
                </p>
                <p className="mt-2 text-3xl font-black">{requiredProgress}%</p>
              </div>
              <div className="rounded-2xl bg-black/10 px-4 py-4">
                <p className="text-[11px] font-black uppercase tracking-[0.25em] text-teal-100">
                  Last result
                </p>
                <p className="mt-2 text-sm font-semibold leading-6">
                  {result?.recommendations?.length
                    ? `${result.recommendations.length} paths ranked`
                    : assessment.hasSavedResponses
                      ? "Saved answers found"
                      : "Take your first test"}
                </p>
              </div>
            </div>
          </div>
        </div>

        <div className="pointer-events-none absolute -right-20 -top-16 h-56 w-56 rounded-full bg-cyan-300/15 blur-3xl"></div>
        <div className="pointer-events-none absolute -left-16 bottom-0 h-48 w-48 rounded-full bg-amber-300/10 blur-3xl"></div>
      </section>

      {(error || notice || inlineMessage) && (
        <div className="space-y-3">
          {error && (
            <div className="rounded-2xl border border-red-100 bg-red-50 px-5 py-4 text-sm font-semibold text-red-600">
              {error}
            </div>
          )}
          {notice && (
            <div className="rounded-2xl border border-emerald-100 bg-emerald-50 px-5 py-4 text-sm font-semibold text-emerald-700">
              {notice}
            </div>
          )}
          {inlineMessage && (
            <div className="rounded-2xl border border-amber-100 bg-amber-50 px-5 py-4 text-sm font-semibold text-amber-700">
              {inlineMessage}
            </div>
          )}
        </div>
      )}

      <div className="grid gap-8 xl:grid-cols-[1.2fr_0.8fr]">
        <section className="rounded-[2.5rem] border border-slate-100 bg-white p-7 shadow-sm sm:p-8">
          <div className="flex flex-col gap-4 border-b border-slate-100 pb-6 sm:flex-row sm:items-end sm:justify-between">
            <div>
              <p className="text-[11px] font-black uppercase tracking-[0.3em] text-[#0f766e]">
                Question {currentIndex + 1} of {totalQuestions}
              </p>
              <h3 className="mt-3 text-2xl font-black tracking-tight text-slate-900">
                {currentQuestion.questionText}
              </h3>
            </div>

            <div className="rounded-2xl bg-slate-50 px-4 py-3 text-sm font-semibold text-slate-500">
              {currentQuestion.isRequired ? "Required" : "Optional"} response
            </div>
          </div>

          <div className="mt-8 space-y-6">
            {currentQuestion.questionType === "text" ? (
              <div className="space-y-3">
                <label className="block text-sm font-bold text-slate-700">
                  Share a real problem space that keeps your attention.
                </label>
                <textarea
                  value={currentValue}
                  onChange={(event) =>
                    setAnswer(currentQuestion.questionId, event.target.value)
                  }
                  rows={7}
                  placeholder="Example: I want to help teams turn messy information into clear decisions, or design products that feel effortless for users."
                  className="min-h-[180px] w-full rounded-[1.75rem] border border-slate-200 bg-slate-50 px-5 py-4 text-sm leading-7 text-slate-700 outline-none transition-all placeholder:text-slate-400 focus:border-[#0f766e] focus:bg-white focus:ring-4 focus:ring-teal-100"
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
                      className={`group rounded-[1.75rem] border px-5 py-5 text-left transition-all ${
                        isSelected
                          ? "border-[#0f766e] bg-[#ecfdf5] shadow-lg shadow-emerald-100"
                          : "border-slate-200 bg-slate-50 hover:-translate-y-0.5 hover:border-slate-300 hover:bg-white hover:shadow-lg hover:shadow-slate-100"
                      }`}
                    >
                      <div className="flex items-start gap-4">
                        <span
                          className={`mt-1 flex h-6 w-6 shrink-0 items-center justify-center rounded-full border text-[11px] font-black transition-all ${
                            isSelected
                              ? "border-[#0f766e] bg-[#0f766e] text-white"
                              : "border-slate-300 bg-white text-slate-400"
                          }`}
                        >
                          {isSelected ? "OK" : currentQuestion.orderNumber}
                        </span>
                        <div>
                          <p
                            className={`text-sm font-bold leading-6 ${
                              isSelected ? "text-slate-900" : "text-slate-700"
                            }`}
                          >
                            {option}
                          </p>
                          <p className="mt-2 text-xs font-semibold uppercase tracking-[0.22em] text-slate-400">
                            {isSelected ? "Selected" : "Choose this direction"}
                          </p>
                        </div>
                      </div>
                    </button>
                  );
                })}
              </div>
            )}
          </div>

          <div className="mt-8 flex flex-col gap-4 border-t border-slate-100 pt-6 sm:flex-row sm:items-center sm:justify-between">
            <div className="flex gap-3">
              <button
                type="button"
                onClick={handlePrevious}
                disabled={currentIndex === 0}
                className="rounded-2xl border border-slate-200 px-5 py-3 text-sm font-black text-slate-500 transition-all hover:border-slate-300 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-45"
              >
                Previous
              </button>
              {currentIndex < totalQuestions - 1 ? (
                <button
                  type="button"
                  onClick={handleNext}
                  className="rounded-2xl bg-slate-900 px-5 py-3 text-sm font-black text-white shadow-lg shadow-slate-200 transition-all hover:-translate-y-0.5 hover:bg-slate-800"
                >
                  Next question
                </button>
              ) : (
                <button
                  type="button"
                  onClick={handleSubmit}
                  disabled={isSubmitting}
                  className="rounded-2xl bg-[#0f766e] px-6 py-3 text-sm font-black text-white shadow-lg shadow-emerald-200 transition-all hover:-translate-y-0.5 hover:bg-[#0b5f59] disabled:cursor-not-allowed disabled:opacity-60"
                >
                  {isSubmitting ? "Analyzing matches..." : "Get my career matches"}
                </button>
              )}
            </div>

            <p className="text-xs font-semibold uppercase tracking-[0.25em] text-slate-400">
              {requiredAnsweredCount}/{requiredTotal} required answered
            </p>
          </div>
        </section>

        <aside className="space-y-6">
          <section className="rounded-[2.25rem] border border-slate-100 bg-white p-6 shadow-sm">
            <div className="flex items-center justify-between">
              <h3 className="text-lg font-black text-slate-900">
                Assessment map
              </h3>
              <span className="rounded-full bg-teal-50 px-3 py-1 text-[11px] font-black uppercase tracking-[0.22em] text-[#0f766e]">
                {overallProgress}% done
              </span>
            </div>

            <div className="mt-5 space-y-3">
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
                    className={`flex w-full items-start gap-4 rounded-2xl px-4 py-3 text-left transition-all ${
                      isActive
                        ? "bg-slate-900 text-white shadow-lg shadow-slate-200"
                        : "bg-slate-50 text-slate-700 hover:bg-slate-100"
                    }`}
                  >
                    <span
                      className={`mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-full text-[11px] font-black ${
                        isActive
                          ? "bg-white text-slate-900"
                          : isAnswered
                            ? "bg-emerald-100 text-emerald-700"
                            : "bg-white text-slate-400"
                      }`}
                    >
                      {isAnswered ? "OK" : index + 1}
                    </span>
                    <div className="min-w-0">
                      <p className="line-clamp-2 text-sm font-bold leading-6">
                        {question.questionText}
                      </p>
                      <p
                        className={`mt-1 text-[11px] font-black uppercase tracking-[0.24em] ${
                          isActive ? "text-slate-300" : "text-slate-400"
                        }`}
                      >
                        {question.isRequired ? "Required" : "Optional"}
                      </p>
                    </div>
                  </button>
                );
              })}
            </div>
          </section>

          <section className="rounded-[2.25rem] border border-slate-100 bg-white p-6 shadow-sm">
            {result ? (
              <div className="space-y-5">
                <div>
                  <p className="text-[11px] font-black uppercase tracking-[0.3em] text-[#0f766e]">
                    Profile signal
                  </p>
                  <h3 className="mt-3 text-xl font-black text-slate-900">
                    What your answers say
                  </h3>
                  <p className="mt-3 text-sm leading-7 text-slate-600">
                    {result.profileSummary}
                  </p>
                </div>

                <div className="flex flex-wrap gap-2">
                  {(result.topTraits || []).map((trait) => (
                    <span
                      key={trait}
                      className="rounded-full bg-[#eefbf8] px-4 py-2 text-xs font-black uppercase tracking-[0.18em] text-[#0f766e]"
                    >
                      {trait}
                    </span>
                  ))}
                </div>

                <div className="rounded-2xl bg-slate-50 p-4">
                  <p className="text-[11px] font-black uppercase tracking-[0.24em] text-slate-400">
                    Ranking logic
                  </p>
                  <p className="mt-2 text-sm leading-7 text-slate-600">
                    {result.recommendationStrategy}
                  </p>
                </div>

                <div className="space-y-3">
                  {(result.responseInsights || []).slice(0, 3).map((insight) => (
                    <div
                      key={insight.questionId}
                      className="rounded-2xl border border-slate-100 bg-white px-4 py-3"
                    >
                      <p className="text-[11px] font-black uppercase tracking-[0.22em] text-slate-400">
                        Insight
                      </p>
                      <p className="mt-2 text-sm font-semibold leading-6 text-slate-700">
                        {insight.aiAnalysis}
                      </p>
                    </div>
                  ))}
                </div>
              </div>
            ) : (
              <div className="space-y-5">
                <div>
                  <p className="text-[11px] font-black uppercase tracking-[0.3em] text-[#0f766e]">
                    How it works
                  </p>
                  <h3 className="mt-3 text-xl font-black text-slate-900">
                    A better way to start when you are unsure
                  </h3>
                </div>

                <div className="space-y-3">
                  <div className="rounded-2xl bg-[#f6fbff] px-4 py-4">
                    <p className="text-sm font-bold text-slate-900">
                      1. Answer with instinct, not perfection
                    </p>
                    <p className="mt-2 text-sm leading-7 text-slate-600">
                      The test is designed to surface the kind of work that
                      energizes you, not to check textbook knowledge.
                    </p>
                  </div>
                  <div className="rounded-2xl bg-[#fff8ec] px-4 py-4">
                    <p className="text-sm font-bold text-slate-900">
                      2. We rank real paths from your database
                    </p>
                    <p className="mt-2 text-sm leading-7 text-slate-600">
                      Recommendations come from the career paths already stored
                      in Path Finder, so users can move directly into action.
                    </p>
                  </div>
                  <div className="rounded-2xl bg-[#f6f8ff] px-4 py-4">
                    <p className="text-sm font-bold text-slate-900">
                      3. Start a path immediately
                    </p>
                    <p className="mt-2 text-sm leading-7 text-slate-600">
                      Once the ranking appears, the user can enroll in the most
                      suitable path without leaving the flow.
                    </p>
                  </div>
                </div>

                <Link
                  to="/ai-assistant"
                  className="inline-flex rounded-2xl border border-slate-200 px-4 py-3 text-sm font-black text-slate-700 transition-all hover:border-slate-300 hover:bg-slate-50"
                >
                  Need extra guidance? Open AI Assistant
                </Link>
              </div>
            )}
          </section>
        </aside>
      </div>

      {result?.recommendations?.length > 0 && (
        <section id="career-match-results" className="space-y-6">
          <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
            <div>
              <p className="text-[11px] font-black uppercase tracking-[0.3em] text-[#0f766e]">
                Ranked matches
              </p>
              <h3 className="mt-3 text-3xl font-black tracking-tight text-slate-900">
                Career paths that fit you best right now
              </h3>
            </div>
            <button
              type="button"
              onClick={() => window.scrollTo({ top: 0, behavior: "smooth" })}
              className="rounded-2xl border border-slate-200 px-5 py-3 text-sm font-black text-slate-600 transition-all hover:border-slate-300 hover:bg-slate-50"
            >
              Review answers
            </button>
          </div>

          <div className="grid gap-6">
            {result.recommendations.map((recommendation, index) => (
              <article
                key={recommendation.careerPathId}
                className="overflow-hidden rounded-[2.5rem] border border-slate-100 bg-white p-7 shadow-sm transition-all hover:-translate-y-0.5 hover:shadow-xl hover:shadow-slate-100 sm:p-8"
              >
                <div className="flex flex-col gap-6 lg:flex-row lg:items-start lg:justify-between">
                  <div className="space-y-4">
                    <div className="flex flex-wrap items-center gap-3">
                      <span className="rounded-full bg-slate-900 px-4 py-2 text-[11px] font-black uppercase tracking-[0.24em] text-white">
                        Rank {index + 1}
                      </span>
                      {recommendation.categoryName && (
                        <span className="rounded-full bg-slate-100 px-4 py-2 text-[11px] font-black uppercase tracking-[0.22em] text-slate-500">
                          {recommendation.subCategoryName
                            ? `${recommendation.categoryName} / ${recommendation.subCategoryName}`
                            : recommendation.categoryName}
                        </span>
                      )}
                    </div>

                    <div>
                      <h4 className="text-2xl font-black tracking-tight text-slate-900">
                        {recommendation.careerPathName}
                      </h4>
                      <p className="mt-3 max-w-3xl text-sm leading-7 text-slate-600">
                        {recommendation.whyItFits}
                      </p>
                    </div>
                  </div>

                  <div className="min-w-[180px] rounded-[2rem] bg-slate-950 px-5 py-5 text-white shadow-lg shadow-slate-200">
                    <p className="text-[11px] font-black uppercase tracking-[0.24em] text-slate-300">
                      Match score
                    </p>
                    <p className="mt-3 text-4xl font-black">
                      {recommendation.suitabilityScore}%
                    </p>
                    <p className="mt-2 text-sm font-semibold text-slate-300">
                      {recommendation.matchReason}
                    </p>
                  </div>
                </div>

                <div className="mt-6 h-3 overflow-hidden rounded-full bg-slate-100">
                  <div
                    className="h-full rounded-full bg-gradient-to-r from-[#0f766e] via-[#14b8a6] to-[#f59e0b]"
                    style={{ width: `${recommendation.suitabilityScore}%` }}
                  ></div>
                </div>

                <div className="mt-6 grid gap-5 lg:grid-cols-2">
                  <div className="rounded-[2rem] bg-[#eefbf8] p-5">
                    <p className="text-[11px] font-black uppercase tracking-[0.24em] text-[#0f766e]">
                      Strength signals
                    </p>
                    <div className="mt-4 flex flex-wrap gap-2">
                      {(recommendation.strengthSignals || []).map((signal) => (
                        <span
                          key={signal}
                          className="rounded-full bg-white px-4 py-2 text-xs font-black uppercase tracking-[0.16em] text-slate-700 shadow-sm"
                        >
                          {signal}
                        </span>
                      ))}
                    </div>
                  </div>

                  <div className="rounded-[2rem] bg-[#fff8ec] p-5">
                    <p className="text-[11px] font-black uppercase tracking-[0.24em] text-[#b45309]">
                      Growth areas
                    </p>
                    <div className="mt-4 flex flex-wrap gap-2">
                      {(recommendation.growthAreas || []).map((area) => (
                        <span
                          key={area}
                          className="rounded-full bg-white px-4 py-2 text-xs font-black uppercase tracking-[0.16em] text-slate-700 shadow-sm"
                        >
                          {area}
                        </span>
                      ))}
                    </div>
                  </div>
                </div>

                <div className="mt-6 flex flex-wrap gap-3">
                  {recommendation.difficultyLevel && (
                    <span className="rounded-full border border-slate-200 px-4 py-2 text-xs font-black uppercase tracking-[0.18em] text-slate-500">
                      {recommendation.difficultyLevel}
                    </span>
                  )}
                  {recommendation.durationInMonths && (
                    <span className="rounded-full border border-slate-200 px-4 py-2 text-xs font-black uppercase tracking-[0.18em] text-slate-500">
                      {recommendation.durationInMonths} months
                    </span>
                  )}
                  <span className="rounded-full border border-slate-200 px-4 py-2 text-xs font-black uppercase tracking-[0.18em] text-slate-500">
                    {recommendation.totalCourses} courses
                  </span>
                </div>

                <div className="mt-6 rounded-[2rem] border border-slate-100 bg-slate-50 px-5 py-5">
                  <p className="text-[11px] font-black uppercase tracking-[0.24em] text-slate-400">
                    Suggested next step
                  </p>
                  <p className="mt-3 text-sm font-semibold leading-7 text-slate-700">
                    {recommendation.suggestedNextStep}
                  </p>
                </div>

                <div className="mt-6 flex flex-col gap-3 sm:flex-row">
                  <button
                    type="button"
                    onClick={() => enrollInPath(recommendation.careerPathId)}
                    disabled={
                      recommendation.isAlreadyEnrolled ||
                      enrollingId === recommendation.careerPathId
                    }
                    className={`rounded-2xl px-5 py-3 text-sm font-black transition-all ${
                      recommendation.isAlreadyEnrolled
                        ? "cursor-not-allowed bg-slate-100 text-slate-400"
                        : "bg-[#0f766e] text-white shadow-lg shadow-emerald-200 hover:-translate-y-0.5 hover:bg-[#0b5f59]"
                    }`}
                  >
                    {recommendation.isAlreadyEnrolled
                      ? "Already in your journey"
                      : enrollingId === recommendation.careerPathId
                        ? "Adding path..."
                        : "Start this path"}
                  </button>

                  <Link
                    to="/ai-assistant"
                    className="rounded-2xl border border-slate-200 px-5 py-3 text-center text-sm font-black text-slate-600 transition-all hover:border-slate-300 hover:bg-slate-50"
                  >
                    Continue with AI Assistant
                  </Link>
                </div>
              </article>
            ))}
          </div>
        </section>
      )}
    </div>
  );
};

export default CareerMatchScreen;
