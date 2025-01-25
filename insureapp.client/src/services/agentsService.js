export const agentsService = {
  async getAllAgents() {
    const response = await axios.get(`${API_URL}/GetAllAgents`);
    return response.data;
  },

  async getUserById(id) {
    const response = await axios.get(`${API_URL}/GetAgentByID/${id}`);
    return response.data;
  },

  async getUserReports(id) {
    const response = await axios.get(`${API_URL}/${id}/reports`);
    return response.data;
  },

  async createUser(userData) {
    const response = await axios.post(`${API_URL}/AddAgentManually`, userData);
    return response.data;
  },

  async updateUser(id, userData) {
    const response = await axios.put(`${API_URL}/EditAgent/${id}`, userData);
    return response.data;
  },

  async deleteUser(id) {
    const response = await axios.delete(`${API_URL}/DeleteAgent/${id}`);
    return response.data;
  }
};
