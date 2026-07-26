import { useEffect, useState, useCallback, useRef } from "react";

// Generic async data hook. `fn` is a factory that returns a Promise.
// `deps` re-runs when they change. Exposes { data, loading, error, refetch }.
export function useAsync(fn, deps = []) {
  const [state, setState] = useState({ data: null, loading: true, error: null });
  const mounted = useRef(true);
  const run = useCallback(async () => {
    setState((s) => ({ ...s, loading: true, error: null }));
    try {
      const data = await fn();
      if (mounted.current) setState({ data, loading: false, error: null });
    } catch (error) {
      if (mounted.current) setState({ data: null, loading: false, error });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, deps);
  useEffect(() => { mounted.current = true; run(); return () => { mounted.current = false; }; }, [run]);
  return { ...state, refetch: run };
}
