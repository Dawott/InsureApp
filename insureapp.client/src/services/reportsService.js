import axios from 'axios';

const API_URL = 'api/InsuranceReports';

export const reportsService = {
  async getAllReports() {
    const response = await axios.get(`${API_URL}/GetAllReports`);
    return response.data;
  },

  async deleteReport(id) {
    const response = await axios.delete(`${API_URL}/DeleteReport/${id}`);
    return response.data;
  },

  async getReportById(id) {
    const response = await axios.get(`${API_URL}/GetReportByID/${id}`);
    return response.data;
  },
  async updateReport(id, reportData) {
    const response = await axios.put(`${API_URL}/EditReport/${id}`, reportData);
    return response.data;
  },

  async createReport(reportData) {
    const response = await axios.post(`${API_URL}/CreateReport`, reportData);
    return response.data;
  }
};
