export interface Denuncia {
  id?: string;
  title: string;
  status: string;
  comment: string;
  success: boolean;
  categoria: string;
  userreport: string;
  userId?: string;
  createdAt?: string;
  mediatorId?: string;
  mediatorName?: string;
}