import axios from 'axios';

const API_URL = '/api/InsuranceAgents';

export const agentsService = {
  async getAllAgents() {
    const response = await axios.get(`${API_URL}/GetAllAgents`);
    return response.data;
  },

  async getAgentById(id) {
    const response = await axios.get(`${API_URL}/GetAgentByID/${id}`);
    return response.data;
  },

  async getAgentReports(id) {
    const response = await axios.get(`${API_URL}/GetReportsForAgent/${id}/reports`);
    return response.data;
  },

  async createAgent(userData) {
    const response = await axios.post(`${API_URL}/AddAgentManually`, userData);
    return response.data;
  },

  async updateAgent(id, userData) {
    const response = await axios.put(`${API_URL}/EditAgent/${id}`, userData);
    return response.data;
  },

  async deleteAgent(id) {
    const response = await axios.delete(`${API_URL}/DeleteAgent/${id}`);
    return response.data;
  }
};
