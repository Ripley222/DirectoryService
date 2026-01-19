import {LocationEntity} from "@/entities/locations/types";
import {apiClient} from "@/shared/api/axios-instance";
import {ApiError} from "@/shared/api/errors";

export type CreateLocationRequest = {
    name: string;
    city: string;
    street: string;
    house: string;
    room_number: string;
    timeZone: string;
}

export type GetLocationsRequest = {
    departmentIds?: string[],
    search?: string,
    isActive?: boolean,
    page?: number,
    pageSize?: number,
    sortBy?: string,
    sortDirection?: string,
}

export type Envelope<T = unknown> = {
    result: T | null;
    error: ApiError | null;
    isError: boolean;
    timeGenerated: string;
}

export const locationsApi = {
    getLocations: async (request: GetLocationsRequest): Promise<LocationEntity[]> => {
        const response = await apiClient
            .get<Envelope<{ locations: LocationEntity[] }>>(
                "locations",
                {
                    params: request
                });

        return response.data.result?.locations || [];
    },

    createLocations: async (request: CreateLocationRequest) => {
        const response = await apiClient.post("locations", request);

        return response.data;
    }
}