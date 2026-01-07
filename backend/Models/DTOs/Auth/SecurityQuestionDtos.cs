namespace ExpenseTracker.Models.DTOs.Auth;

/// <summary>
/// Security question for registration
/// </summary>
public class SecurityQuestionDto
{
    public required int QuestionId { get; set; }
    public required string Answer { get; set; }
}

/// <summary>
/// Extended registration request with security questions
/// </summary>
public class AuthRegisterWithSecurityRequest : AuthRegisterRequest
{
    /// <summary>
    /// List of 3 security question answers
    /// </summary>
    public required List<SecurityQuestionDto> SecurityQuestions { get; set; }
}

/// <summary>
/// Response with available security questions
/// </summary>
public class SecurityQuestionsListResponse
{
    public required List<SecurityQuestionItem> Questions { get; set; }
}

public class SecurityQuestionItem
{
    public int Id { get; set; }
    public required string Question { get; set; }
}

/// <summary>
/// Request to initiate forgot password flow
/// </summary>
public class ForgotPasswordInitRequest
{
    public required string Email { get; set; }
}

/// <summary>
/// Response with user's security questions (without answers)
/// </summary>
public class ForgotPasswordQuestionsResponse
{
    public required string Email { get; set; }
    public required List<UserSecurityQuestion> Questions { get; set; }
}

public class UserSecurityQuestion
{
    public int QuestionOrder { get; set; }
    public int QuestionId { get; set; }
    public required string Question { get; set; }
}

/// <summary>
/// Request to verify security answers and reset password
/// </summary>
public class ResetPasswordRequest
{
    public required string Email { get; set; }
    public required List<SecurityQuestionDto> Answers { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmNewPassword { get; set; }
}
