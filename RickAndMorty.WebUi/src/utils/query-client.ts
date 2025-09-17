import { QueryClient } from "@tanstack/react-query";

const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            retry: 2,
            staleTime: 1000 * 2,
            gcTime: 1000 * 1.9,
        },
    },
});

export default queryClient;