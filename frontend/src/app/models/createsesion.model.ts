export interface CreateSessionRequest {
  caseId: string;
  scheduledDate: string;
  modality: string;
  meetingLink?: string;
}