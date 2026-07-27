// E15: chính sách mật khẩu — tối thiểu 8 ký tự, gồm chữ hoa, chữ thường và số
export function validatePassword(pwd) {
  if (!pwd || pwd.length < 8) return false;
  return /[A-Z]/.test(pwd) && /[a-z]/.test(pwd) && /[0-9]/.test(pwd);
}

export const PASSWORD_RULE_TEXT =
  'Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường và số';
