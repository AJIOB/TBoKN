#include <bitset>

#include "MyLib.h"

const int messageLengthInBytes = 2;
const int bitsInByte = 8;
const int numOfControlBits = 5;


std::deque<bool> EncodeMessage(std::string message)
{
	if (message.length() != messageLengthInBytes)
	{
		throw std::exception("First argument must contains 2 symbols only");
	}

	std::deque<bool> result = BytesToBits(message);

	//TODO

	return result;
}

std::string DecodeMessage(std::deque<bool> message, int& numOfFoundErrors)
{
	//TODO
	return std::string();
}

std::deque<bool> BytesToBits(std::string bytes)
{
	std::deque<bool> result;
	for (auto c : bytes)
	{
		std::bitset<bitsInByte> bitset(c);
		for (auto i = 0U; i < bitset.size(); ++i)
		{
			result.push_back(bitset[i]);
		}
	}

	return result;
}

std::string BitsToBytes(std::deque<bool> bits)
{
	std::string result;

	//TODO

	return result;
}
