import requests
import random
import json
import urllib3

# Disable warnings for unverified HTTPS requests.
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

# Base URL for the tournaments API home route.
tournaments_api_url = 'https://localhost:5001/tournaments-api'

def get_api_info():
    """
    Retrieves basic configuration information and endpoint links
    from the tournaments API home route.
    """
    try:
        resp = requests.get(tournaments_api_url, verify=False)
        resp.raise_for_status()
        result = resp.json()
        # The API home returns a set of links. We expect at least an "all-tournaments" link.
        tournaments_url = result['links']['all-tournaments']['href']
        # The registration link is provided as a template; we'll substitute the tournament id.
        register_template = result['links']['register']['href']
        # The game details link template.
        game_details_template = result['links']['game-details']['href']
        return {
            'tournaments_url': tournaments_url,
            'register_template': register_template,
            'game_details_template': game_details_template
        }
    except Exception as e:
        print("Error retrieving API info:", e)
        return None

def get_all_tournaments(tournaments_url):
    """
    Retrieves a list of all tournaments.
    """
    try:
        resp = requests.get(tournaments_url, verify=False)
        if resp.status_code == 200:
            data = resp.json()
            tournaments = data.get('tournaments', [])
            result = {
                'success': True,
                'tournaments': tournaments,
                'message': f"Retrieved {len(tournaments)} tournaments." if tournaments else "No tournaments available."
            }
        else:
            result = {
                'success': False,
                'tournaments': [],
                'message': "Error retrieving tournaments."
            }
    except Exception as e:
        result = {
            'success': False,
            'tournaments': [],
            'message': f"Exception retrieving tournaments: {str(e)}"
        }
    return result

def get_games_for_tournament(tournament_id):
    """
    Retrieves all games for a specified tournament.
    """
    # The API endpoint is of the form /tournaments-api/tournaments/{tournamentId}/games
    games_url = f"{tournaments_api_url}/tournaments/{tournament_id}/games"
    try:
        resp = requests.get(games_url, verify=False)
        if resp.status_code == 200:
            data = resp.json()
            games = data.get('games', [])
            result = {
                'success': True,
                'games': games,
                'message': f"Retrieved {len(games)} game(s) for tournament {tournament_id}." if games else f"No games found for tournament {tournament_id}."
            }
        else:
            result = {
                'success': False,
                'games': [],
                'message': f"Error retrieving games for tournament {tournament_id}."
            }
    except Exception as e:
        result = {
            'success': False,
            'games': [],
            'message': f"Exception retrieving games: {str(e)}"
        }
    return result

def register_for_tournament(tournament_id, name, email):
    """
    Submits a registration (name and email) for the specified tournament.
    """
    reg_url = f"{tournaments_api_url}/tournaments/{tournament_id}/register"
    payload = {"name": name, "email": email}
    headers = {"Content-Type": "application/json"}
    try:
        resp = requests.post(reg_url, headers=headers, json=payload, verify=False)
        if resp.status_code == 201:
            result = {
                'success': True,
                'registration': resp.json(),
                'message': "Registration successful."
            }
        else:
            result = {
                'success': False,
                'message': f"Registration failed with status {resp.status_code}."
            }
    except Exception as e:
        result = {
            'success': False,
            'message': f"Exception during registration: {str(e)}"
        }
    return result

def get_game_details(game_id):
    """
    Retrieves detailed information for a particular game.
    """
    url = f"{tournaments_api_url}/games/{game_id}"
    try:
        resp = requests.get(url, verify=False)
        if resp.status_code == 200:
            result = {
                'success': True,
                'game': resp.json(),
                'message': f"Retrieved details for game {game_id}."
            }
        else:
            result = {
                'success': False,
                'game': None,
                'message': f"Error retrieving details for game {game_id}."
            }
    except Exception as e:
        result = {
            'success': False,
            'game': None,
            'message': f"Exception retrieving game details: {str(e)}"
        }
    return result

def summarize_tournament(tournament):
    """Formats tournament details into a string for printing."""
    return (f"Tournament ID: {tournament.get('tournamentId')}, "
            f"Name: {tournament.get('name')}, "
            f"Game: {tournament.get('game')}, "
            f"Players per team: {tournament.get('numPlayersPerTeam')}, "
            f"Start: {tournament.get('startDateTime')}, "
            f"Games: {tournament.get('numGames')}")

def summarize_game(game):
    """Formats game details into a string for printing."""
    team_names = ", ".join(game.get('teamNames', []))
    return (f"Game ID: {game.get('gameId')}, "
            f"Date/Time: {game.get('gameDateTime')}, "
            f"Result: {game.get('result')}, "
            f"Teams: {team_names}")

# Main client loop.
def main():
    api_info = get_api_info()
    if not api_info:
        print("Failed to load API configuration. Exiting.")
        return

    tournaments_url = api_info['tournaments_url']

    main_options = [
        "List all tournaments",
        "List games for a specific tournament",
        "Register for a tournament",
        "Get details for a game",
        "Quit"
    ]

    done = False
    while not done:
        print("\n" + "\n".join([f"{i+1}). {main_options[i]}" for i in range(len(main_options))]))
        try:
            choice = int(input("\nWhat do you want to do? (enter number): "))
        except ValueError:
            print("Please enter a valid number.")
            continue

        if choice == 1:
            result = get_all_tournaments(tournaments_url)
            print("\n" + result.get("message", ""))
            if result['success']:
                for t in result['tournaments']:
                    print(summarize_tournament(t))
            else:
                print("Could not list tournaments.")

        elif choice == 2:
            try:
                tid = int(input("Enter the tournament id: ").strip())
            except ValueError:
                print("Invalid tournament id.")
                continue
            result = get_games_for_tournament(tid)
            print("\n" + result.get("message", ""))
            if result['success']:
                for g in result['games']:
                    print(summarize_game(g))
            else:
                print("Could not retrieve games.")

        elif choice == 3:
            try:
                tid = int(input("Enter the tournament id for registration: ").strip())
            except ValueError:
                print("Invalid tournament id.")
                continue
            name = input("Your name: ").strip()
            email = input("Your email: ").strip()
            result = register_for_tournament(tid, name, email)
            print("\n" + result.get("message", ""))
            if result['success']:
                registration = result.get("registration")
                print("Registration details:")
                print(json.dumps(registration, indent=2))
        elif choice == 4:
            try:
                gid = int(input("Enter the game id: ").strip())
            except ValueError:
                print("Invalid game id.")
                continue
            result = get_game_details(gid)
            print("\n" + result.get("message", ""))
            if result['success']:
                game = result.get("game")
                print(summarize_game(game))
            else:
                print("Could not retrieve game details.")

        elif choice == 5:
            print("\nGoodbye!\n")
            done = True
        else:
            print("Invalid option, please try again.")

if __name__ == "__main__":
    main()