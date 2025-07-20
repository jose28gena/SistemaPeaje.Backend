#!/usr/bin/env python3
"""
Simulador PLC simple para testing de reconexión
Simula estados de coils y permite desconexión/reconexión
"""

import socket
import threading
import time
import struct
import random

class SimplePLCSimulator:
    def __init__(self, host='127.0.0.1', port=502):
        self.host = host
        self.port = port
        self.running = False
        self.server_socket = None
        
        # Estados de los coils (7 coils desde dirección 0)
        self.coils = [False] * 7
        
        # Configurar algunos valores iniciales interesantes
        self.coils[0] = False  # Presencia
        self.coils[1] = True   # BarreraAbierta
        self.coils[2] = True   # SentidoAB
        self.coils[3] = False  # Alarma
        self.coils[4] = False  # PresenciaSalida
        self.coils[5] = False  # BarreraSalida
        self.coils[6] = False  # AlarmaSalida
        
    def start(self):
        """Inicia el servidor PLC"""
        try:
            self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
            self.server_socket.bind((self.host, self.port))
            self.server_socket.listen(5)
            self.running = True
            
            print(f"🔌 PLC Simulador iniciado en {self.host}:{self.port}")
            print("📊 Estados iniciales de coils:")
            self.print_coil_states()
            
            while self.running:
                try:
                    client_socket, address = self.server_socket.accept()
                    print(f"🔗 Cliente conectado desde {address}")
                    
                    # Manejar cliente en hilo separado
                    client_thread = threading.Thread(
                        target=self.handle_client,
                        args=(client_socket,)
                    )
                    client_thread.daemon = True
                    client_thread.start()
                    
                except socket.error:
                    if self.running:
                        print("❌ Error en el socket del servidor")
                    break
                    
        except Exception as e:
            print(f"❌ Error iniciando servidor: {e}")
        finally:
            self.stop()
    
    def stop(self):
        """Detiene el servidor PLC"""
        self.running = False
        if self.server_socket:
            self.server_socket.close()
        print("🔌 PLC Simulador detenido")
    
    def handle_client(self, client_socket):
        """Maneja las peticiones de un cliente"""
        try:
            while self.running:
                # Leer petición Modbus TCP
                data = client_socket.recv(1024)
                if not data:
                    break
                
                # Procesar petición
                response = self.process_modbus_request(data)
                if response:
                    client_socket.send(response)
                    
        except socket.error:
            pass
        finally:
            client_socket.close()
            print("🔗 Cliente desconectado")
    
    def process_modbus_request(self, data):
        """Procesa una petición Modbus TCP"""
        if len(data) < 8:
            return None
        
        # Header Modbus TCP: Transaction ID (2) + Protocol ID (2) + Length (2) + Unit ID (1) + Function Code (1)
        transaction_id = struct.unpack('>H', data[0:2])[0]
        protocol_id = struct.unpack('>H', data[2:4])[0]
        length = struct.unpack('>H', data[4:6])[0]
        unit_id = data[6]
        function_code = data[7]
        
        # Solo soportamos función 1 (Read Coils)
        if function_code == 1:
            if len(data) < 12:
                return None
                
            start_address = struct.unpack('>H', data[8:10])[0]
            coil_count = struct.unpack('>H', data[10:12])[0]
            
            return self.read_coils_response(transaction_id, unit_id, start_address, coil_count)
        
        return None
    
    def read_coils_response(self, transaction_id, unit_id, start_address, coil_count):
        """Genera respuesta para lectura de coils"""
        # Calcular cuántos bytes necesitamos para los coils
        byte_count = (coil_count + 7) // 8
        
        # Convertir coils a bytes
        coil_bytes = []
        for i in range(byte_count):
            byte_value = 0
            for j in range(8):
                coil_index = start_address + (i * 8) + j
                if coil_index < len(self.coils) and coil_index < start_address + coil_count:
                    if self.coils[coil_index]:
                        byte_value |= (1 << j)
            coil_bytes.append(byte_value)
        
        # Construir respuesta Modbus TCP
        response_length = 3 + byte_count  # Unit ID + Function Code + Byte Count + Data
        
        response = struct.pack('>H', transaction_id)  # Transaction ID
        response += struct.pack('>H', 0)              # Protocol ID
        response += struct.pack('>H', response_length) # Length
        response += struct.pack('B', unit_id)         # Unit ID
        response += struct.pack('B', 1)               # Function Code
        response += struct.pack('B', byte_count)      # Byte Count
        
        for byte_val in coil_bytes:
            response += struct.pack('B', byte_val)
        
        return response
    
    def print_coil_states(self):
        """Imprime el estado actual de los coils"""
        names = ['Presencia', 'BarreraAbierta', 'SentidoAB', 'Alarma', 
                'PresenciaSalida', 'BarreraSalida', 'AlarmaSalida']
        
        states = []
        for i, name in enumerate(names):
            if i < len(self.coils):
                states.append(f"{name}: {self.coils[i]}")
        
        print("📊 " + ", ".join(states))
    
    def toggle_coil(self, index):
        """Cambia el estado de un coil"""
        if 0 <= index < len(self.coils):
            self.coils[index] = not self.coils[index]
            self.print_coil_states()
    
    def simulate_vehicle_detection(self):
        """Simula detección de vehículo"""
        print("🚗 Simulando detección de vehículo...")
        self.coils[0] = True  # Presencia = True
        self.print_coil_states()
        
        # Después de 3 segundos, quitar presencia
        threading.Timer(3.0, self.clear_vehicle).start()
    
    def clear_vehicle(self):
        """Quita la presencia del vehículo"""
        print("🚗 Vehículo salió...")
        self.coils[0] = False  # Presencia = False
        self.print_coil_states()

def main():
    simulator = SimplePLCSimulator()
    
    # Iniciar servidor en hilo separado
    server_thread = threading.Thread(target=simulator.start)
    server_thread.daemon = True
    server_thread.start()
    
    print("\n🎮 Comandos disponibles:")
    print("  q - Salir")
    print("  s - Mostrar estados")
    print("  0-6 - Toggle coil (0=Presencia, 1=BarreraAbierta, etc.)")
    print("  v - Simular detección de vehículo")
    print("  r - Reiniciar servidor (simular reconexión)")
    
    try:
        while True:
            command = input("\n> ").strip().lower()
            
            if command == 'q':
                break
            elif command == 's':
                simulator.print_coil_states()
            elif command == 'v':
                simulator.simulate_vehicle_detection()
            elif command == 'r':
                print("🔄 Reiniciando servidor...")
                simulator.stop()
                time.sleep(2)
                simulator = SimplePLCSimulator()
                server_thread = threading.Thread(target=simulator.start)
                server_thread.daemon = True
                server_thread.start()
                time.sleep(1)
            elif command.isdigit():
                coil_index = int(command)
                if 0 <= coil_index <= 6:
                    simulator.toggle_coil(coil_index)
                else:
                    print("❌ Coil index debe estar entre 0-6")
            else:
                print("❌ Comando no reconocido")
                
    except KeyboardInterrupt:
        pass
    finally:
        simulator.stop()
        print("👋 ¡Adiós!")

if __name__ == "__main__":
    main()
