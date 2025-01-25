import axios from 'axios';

const API_URL = '/api/EndUsers';

export const usersService = {
  async getAllUsers() {
    const response = await axios.get(`${API_URL}/GetAllCustomers`);
    return response.data;
  },

  async getUserById(id) {
    const response = await axios.get(`${API_URL}/GetUserByID/${id}`);
    return response.data;
  },

  async getUserReports(id) {
    const response = await axios.get(`${API_URL}/ShowReportsOf/${id}/reports`);
    return response.data;
  },

  async createUser(userData) {
    const response = await axios.post(`${API_URL}/AddUserManually`, userData);
    return response.data;
  },

  async updateUser(id, userData) {
    const response = await axios.put(`${API_URL}/EditUser/${id}`, userData);
    return response.data;
  },

  async deleteUser(id) {
    const response = await axios.delete(`${API_URL}/DeleteUser/${id}`);
    return response.data;
  }
};
