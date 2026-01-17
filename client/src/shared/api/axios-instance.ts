import axios from "axios";
import {Envelope} from "@/entities/locations/api";
import {EnvelopeError} from "@/shared/api/errors";

export const apiClient = axios.create({
    baseURL: "http://localhost:9001/api",
    headers: {"Content-Type": "application/json"}
})

apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (axios.isAxiosError(error) && error.response?.data) {
            const envelope = error.response.data as Envelope;

            if (envelope.error) {
                throw new EnvelopeError(envelope.error);
            }
        }

        return Promise.reject(error);
    }
)