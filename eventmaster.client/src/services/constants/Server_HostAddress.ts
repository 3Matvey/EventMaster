// ����������� �������� (��� ����������)
export let Server_HostAddress: string = 'http://localhost:5242';

// ������� ��� ���������� ����������
export function updateServerHostAddress(newAddress: string) {
    Server_HostAddress = newAddress;
}
