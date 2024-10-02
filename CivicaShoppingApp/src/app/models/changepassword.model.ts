export interface ChangePasswordModel{
    loginId : string | null | undefined;
    oldPassword : string;
    newPassword : string;
    newConfirmPassword : string;
}