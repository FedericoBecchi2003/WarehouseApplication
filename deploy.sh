#!/bin/bash

# ========================================
# Warehouse Application Deployment Helper
# ========================================

set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Functions
print_header() {
    echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ $1${NC}"
}

# Check prerequisites
check_prerequisites() {
    print_header "Checking Prerequisites"
    
    local missing=0
    
    if ! command -v docker &> /dev/null; then
        print_error "Docker not found"
        missing=1
    else
        print_success "Docker installed"
    fi
    
    if ! command -v docker compose &> /dev/null; then
        print_error "Docker Compose not found"
        missing=1
    else
        print_success "Docker Compose installed"
    fi
    
    if [ $missing -eq 1 ]; then
        print_error "Please install missing dependencies"
        exit 1
    fi
}

# Setup .env file
setup_env() {
    print_header "Setting Up Environment"
    
    if [ ! -f .env ]; then
        print_info "Creating .env from template..."
        cp .env.example .env
        print_success ".env file created"
        print_warning "⚠️  Please edit .env and add your credentials:"
        print_info "   - DOCKERHUB_USERNAME"
        print_info "   - DOCKERHUB_TOKEN"
        print_info "   - GITHUB_TOKEN"
        print_warning "Then run the script again"
        return
    fi
    
    print_success ".env file found and will be used"
    
    # Verify that it's not just the template
    if grep -q "your_dockerhub_username" .env; then
        print_error ".env still contains placeholder values!"
        print_warning "Please edit .env and replace:"
        print_info "   DOCKERHUB_USERNAME=your_dockerhub_username"
        print_info "   DOCKERHUB_TOKEN=your_dockerhub_token"
        print_info "   GITHUB_TOKEN=your_github_pat_token"
        return
    fi
    
    print_success "Environment ready to use"
}

# Build services
build_services() {
    print_header "Building Services"
    
    source .env 2>/dev/null || { print_error "Could not source .env"; return 1; }
    
    if [ -z "$GITHUB_TOKEN" ]; then
        print_warning "⚠️  GITHUB_TOKEN non configurato in .env"
        print_info "Per NuGet packages private, configura il token"
    fi
    
    print_info "Building with docker-compose..."
    docker compose build
    
    print_success "All services built successfully"
}

# Start services
start_services() {
    print_header "Starting Services"
    
    docker compose up -d
    
    print_success "Services started"
    print_info "Waiting for services to be ready..."
    sleep 10
    
    docker compose ps
}

# Stop services
stop_services() {
    print_header "Stopping Services"
    
    docker compose down
    
    print_success "Services stopped"
}

# View logs
view_logs() {
    local service=$1
    
    if [ -z "$service" ]; then
        docker compose logs -f
    else
        docker compose logs -f $service
    fi
}

# Health check
health_check() {
    print_header "Performing Health Checks"
    
    echo -n "Order Service: "
    if curl -s http://localhost:5001/health > /dev/null; then
        print_success "OK"
    else
        print_error "NOT RESPONDING"
    fi
    
    echo -n "Payment Service: "
    if curl -s http://localhost:5003/health > /dev/null; then
        print_success "OK"
    else
        print_error "NOT RESPONDING"
    fi
    
    echo -n "Warehouse Service: "
    if curl -s http://localhost:5002/health > /dev/null; then
        print_success "OK"
    else
        print_error "NOT RESPONDING"
    fi
}

# Clean up
cleanup() {
    print_header "Cleaning Up"
    
    read -p "Do you want to remove containers and volumes? (y/n) " -n 1 -r
    echo
    
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        docker compose down -v
        print_success "Cleanup completed"
    else
        print_info "Cleanup cancelled"
    fi
}

# Push to DockerHub
push_to_dockerhub() {
    print_header "Pushing to DockerHub"
    
    source .env 2>/dev/null || { print_error "Could not source .env"; return 1; }
    
    read -p "Are you sure? This will push to DockerHub. (y/n) " -n 1 -r
    echo
    
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        print_info "Push cancelled"
        return
    fi
    
    docker login -u $DOCKERHUB_USERNAME
    
    print_info "Tagging images..."
    docker tag warehouse-order-service:local $DOCKERHUB_USERNAME/warehouse-order-service:latest
    docker tag warehouse-payment-service:local $DOCKERHUB_USERNAME/warehouse-payment-service:latest
    docker tag warehouse-warehouse-service:local $DOCKERHUB_USERNAME/warehouse-warehouse-service:latest
    
    print_info "Pushing images..."
    docker push $DOCKERHUB_USERNAME/warehouse-order-service:latest &
    docker push $DOCKERHUB_USERNAME/warehouse-payment-service:latest &
    docker push $DOCKERHUB_USERNAME/warehouse-warehouse-service:latest &
    
    wait
    
    print_success "All images pushed successfully"
}

# Display menu
show_menu() {
    echo ""
    echo -e "${BLUE}Warehouse Application Deployment Menu${NC}"
    echo "======================================="
    echo "1) Check prerequisites"
    echo "2) Setup environment (.env)"
    echo "3) Build services (local)"
    echo "4) Start services"
    echo "5) Stop services"
    echo "6) View logs"
    echo "7) Health check"
    echo "8) Push to DockerHub"
    echo "9) Full setup (1-4)"
    echo "10) Cleanup"
    echo "0) Exit"
    echo "======================================="
    echo ""
}

# Main
if [ $# -eq 0 ]; then
    print_info "No arguments provided. Running interactive mode..."
    echo ""
    
    while true; do
        show_menu
        read -p "Enter your choice [0-10]: " choice
        
        case $choice in
            1) check_prerequisites ;;
            2) setup_env ;;
            3) build_services ;;
            4) start_services ;;
            5) stop_services ;;
            6) 
                read -p "Enter service name (or press enter for all): " service
                view_logs $service
                ;;
            7) health_check ;;
            8) push_to_dockerhub ;;
            9) 
                check_prerequisites
                setup_env
                build_services
                start_services
                health_check
                ;;
            10) cleanup ;;
            0) 
                print_info "Exiting..."
                exit 0
                ;;
            *)
                print_error "Invalid option"
                ;;
        esac
        
        echo ""
        read -p "Press enter to continue..."
    done
else
    case $1 in
        check) check_prerequisites ;;
        setup) setup_env ;;
        build) build_services ;;
        start) start_services ;;
        stop) stop_services ;;
        logs) view_logs $2 ;;
        health) health_check ;;
        push) push_to_dockerhub ;;
        clean) cleanup ;;
        *)
            echo "Usage: $0 {check|setup|build|start|stop|logs|health|push|clean}"
            exit 1
            ;;
    esac
fi
