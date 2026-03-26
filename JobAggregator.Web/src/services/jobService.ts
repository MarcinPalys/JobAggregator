import axios from 'axios';
import type { JobOffer } from '../types/JobOffer';

const API_URL = 'http://localhost:52334/api';

export interface PagedResult {
  items: JobOffer[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export const jobService = {
  getAll: async (): Promise<JobOffer[]> => {
    const response = await axios.get<JobOffer[]>(`${API_URL}/Jobs`);
    return response.data;
  },

  getPaged: async (page: number, pageSize: number = 20): Promise<PagedResult> => {
    const response = await axios.get<PagedResult>(
      `${API_URL}/Jobs/paged?page=${page}&pageSize=${pageSize}`
    );
    return response.data;
  },

  getById: async (id: string): Promise<JobOffer> => {
    const response = await axios.get<JobOffer>(`${API_URL}/Jobs/${id}`);
    return response.data;
  }
};