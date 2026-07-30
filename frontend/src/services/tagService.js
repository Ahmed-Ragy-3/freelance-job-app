import { staticData, adminService } from "./index";
export const tagService = {
  list: () => staticData.tags(),
  admin: () => adminService.tags(),
};
export default tagService;
