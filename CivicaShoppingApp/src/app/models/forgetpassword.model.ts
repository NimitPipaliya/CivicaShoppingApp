export interface ForgetPasswordModel {
    loginId: string | null | undefined;
    securityQuestionId: number;
    answer: string;
    newPassword: string;
    confirmNewPassword: string;
}