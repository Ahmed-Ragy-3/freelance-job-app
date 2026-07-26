import { staticData, adminService } from "./index";
export const skillService = {
  list: () => staticData.skills(),
  admin: () => adminService.skills(),
};
export default skillService;
