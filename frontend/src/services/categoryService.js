import { staticData, adminService } from "./index";
export const categoryService = {
  list: () => staticData.categories(),
  admin: () => adminService.categories(),
};
export default categoryService;
