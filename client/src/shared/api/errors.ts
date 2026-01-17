export type ApiError = {
    messages: ErrorMassage[];
    type: ErrorType;
}

export type ErrorMassage = {
    code: string;
    message: string;
    invalidField?: string | null;
}

export type ErrorType =
    | "validation"
    | "not_found"
    | "failure"
    | "conflict";

export class EnvelopeError extends Error {
    public readonly apiError: ApiError;
    public readonly type: ErrorType;

    constructor(apiError: ApiError) {
        const firstMessage = apiError.messages[0].message ?? "Неизвестная ошибка";

        super(firstMessage);

        this.name = "EnvelopeError";
        this.apiError = apiError;
        this.type = apiError.type;

        Object.setPrototypeOf(this, EnvelopeError.prototype);
    }

    get messages(): ErrorMassage[] {
        return this.apiError.messages;
    }

    get firstMessage(): string {
        return this.apiError.messages[0].message ?? "Неизвестная ошибка";
    }

    get allMessages(): string[] {
        return this.apiError.messages.map((msg) => msg.message);
    }
}

export function isEnvelopeError(error: unknown): error is EnvelopeError {
    return error instanceof EnvelopeError;
}