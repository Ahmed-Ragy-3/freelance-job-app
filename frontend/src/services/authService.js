// Individual service module — re-exports the same API so components can
// import from a single, backend-shaped path. Swap the internals for real
// API calls (see ./api.js) without changing any import in the app.
export { authService as default, authService } from "./index";
