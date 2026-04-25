import { useEffect, useState } from "react";
import { careerMatchService } from "../services/careerMatchService";

const getErrorMessage = (error, fallback) =>
  error?.response?.data?.message ||
  error?.response?.data?.Message ||
  error?.message ||
  fallback;

export const useCareerMatch = () => {
  const [assessment, setAssessment] = useState(null);
  const [answers, setAnswers] = useState({});
  const [result, setResult] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [enrollingId, setEnrollingId] = useState(null);
  const [error, setError] = useState("");
  const [notice, setNotice] = useState("");

  const loadAssessment = async () => {
    setIsLoading(true);
    setError("");

    try {
      const data = await careerMatchService.getAssessment();
      setAssessment(data);

      const nextAnswers = {};
      (data?.questions || []).forEach((question) => {
        if (question?.savedAnswer) {
          nextAnswers[question.questionId] = question.savedAnswer;
        }
      });

      setAnswers(nextAnswers);
    } catch (requestError) {
      setError(
        getErrorMessage(
          requestError,
          "Unable to load the career assessment right now."
        )
      );
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadAssessment();
  }, []);

  const setAnswer = (questionId, value) => {
    setNotice("");
    setError("");

    setAnswers((currentAnswers) => ({
      ...currentAnswers,
      [questionId]: value,
    }));
  };

  const submitAssessment = async () => {
    if (!assessment?.questions?.length) {
      return null;
    }

    setIsSubmitting(true);
    setError("");
    setNotice("");

    try {
      const payloadAnswers = assessment.questions.map((question) => ({
        questionId: question.questionId,
        answer: answers[question.questionId] ?? "",
      }));

      const data = await careerMatchService.submitAssessment(
        assessment.questionnaireId,
        payloadAnswers
      );

      setResult(data);
      setNotice("Your latest answers were analyzed successfully.");
      return data;
    } catch (requestError) {
      setError(
        getErrorMessage(
          requestError,
          "We could not analyze your answers yet."
        )
      );
      return null;
    } finally {
      setIsSubmitting(false);
    }
  };

  const enrollInPath = async (careerPathId) => {
    setEnrollingId(careerPathId);
    setError("");
    setNotice("");

    try {
      await careerMatchService.enrollInCareerPath(careerPathId);

      setResult((currentResult) => {
        if (!currentResult) {
          return currentResult;
        }

        return {
          ...currentResult,
          recommendations: (currentResult.recommendations || []).map(
            (recommendation) =>
              recommendation.careerPathId === careerPathId
                ? { ...recommendation, isAlreadyEnrolled: true }
                : recommendation
          ),
        };
      });

      setNotice("The career path was added to your journey.");
      return true;
    } catch (requestError) {
      setError(
        getErrorMessage(
          requestError,
          "We could not enroll you in that career path."
        )
      );
      return false;
    } finally {
      setEnrollingId(null);
    }
  };

  const questions = assessment?.questions || [];
  const requiredQuestions = questions.filter((question) => question.isRequired);

  const answeredQuestions = questions.filter((question) => {
    const value = answers[question.questionId];
    return typeof value === "string" ? value.trim().length > 0 : Boolean(value);
  });

  const answeredRequiredQuestions = requiredQuestions.filter((question) => {
    const value = answers[question.questionId];
    return typeof value === "string" ? value.trim().length > 0 : Boolean(value);
  });

  return {
    assessment,
    answers,
    result,
    questions,
    totalQuestions: questions.length,
    answeredCount: answeredQuestions.length,
    requiredTotal: requiredQuestions.length,
    requiredAnsweredCount: answeredRequiredQuestions.length,
    isLoading,
    isSubmitting,
    enrollingId,
    error,
    notice,
    setAnswer,
    submitAssessment,
    enrollInPath,
    refreshAssessment: loadAssessment,
  };
};
