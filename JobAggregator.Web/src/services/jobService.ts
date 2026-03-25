import axios from 'axios';
import type { JobOffer } from '../types/JobOffer';

const API_URL = 'http://localhost:52334/api';

export const jobService = {
  getAll: async (): Promise<JobOffer[]> => {
    const response = await axios.get<JobOffer[]>(`${API_URL}/Jobs`);
    return response.data;
  },

  getById: async (id: string): Promise<JobOffer> => {
    const response = await axios.get<JobOffer>(`${API_URL}/Jobs/${id}`);
    return response.data;
  }
};