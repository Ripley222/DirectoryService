import {useQuery} from "@tanstack/react-query";
import {locationsApi} from "@/entities/locations/api";
import {EnvelopeError} from "@/shared/api/errors";

const PAGE_SIZE = 10;

export function useLocations(page: number) {
    const {data, isPending, isError, error} = useQuery({
        queryFn: () => locationsApi.getLocations({page: page, pageSize: PAGE_SIZE}),
        queryKey: ["locations"]
    });

    return{
        locations: data,
        isPending,
        isError,
        error: error instanceof EnvelopeError ? error : undefined,
    }
}