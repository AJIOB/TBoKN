#include <bitset>
#include <random>
#include <chrono>

#include "MyLib.h"

const int messageLengthInBytes = 2;
const int bitsInByte = 8;
const int numOfControlBits = 5;
const int maxErrors = 1;

std::deque<bool> EncodeMessage(std::string message)
{
	if (message.length() != messageLengthInBytes)
	{
		throw std::exception("First argument must contains 2 symbols only");
	}

	std::deque<bool> result = BytesToBits(message);
	//for main indexing from 1
	result.push_front(false);

	//inserting control bits
	for (auto i = 0; i < numOfControlBits; ++i)
	{
		//1 << i == pow(2, i)
		result.insert(result.begin() + (1 << i), false);
	}

	for (auto i = 0; i < numOfControlBits; ++i)
	{
		//1 << i == pow(2, i)
		result[1 << i] = CalculateControlBit(result, i);
	}

	//removing indexing from 1
	result.pop_front();
	return result;
}

std::string DecodeMessage(std::deque<bool> message, int& numOfFoundErrors, int& posOfError)
{
	//for main indexing from 1
	message.push_front(false);

	if (!FindAndRemoveErrorsIfCan(message, numOfFoundErrors, posOfError))
	{
		return "";
	}

	//removing control bits
	for (auto i = numOfControlBits - 1; i >= 0; --i)
	{
		//1 << i == pow(2, i)
		message.erase(message.begin() + (1 << i));
	}

	//removing indexing from 1
	message.pop_front();
	return BitsToBytes(message);
}

std::deque<bool> MakeErrors(std::deque<bool> bits, int& lastPosOfMadeError)
{
	// configuring random generator
	const auto seed = std::chrono::system_clock::now().time_since_epoch().count();
	std::default_random_engine generator(static_cast<unsigned int> (seed));
	const std::uniform_int_distribution<int> distribution(0, bits.size() - 1);

	for (auto i = 0U; i < maxErrors; ++i)
	{
		lastPosOfMadeError = distribution(generator);
		bits[lastPosOfMadeError] = !bits[lastPosOfMadeError];
	}

	return bits;
}

bool FindAndRemoveErrorsIfCan(std::deque<bool>& message, int& numOfFoundErrors, int& posOfError)
{
	int errorPos = 0;
	for (auto i = 0; i < numOfControlBits; ++i)
	{
		if (CalculateControlBit(message, i))
		{
			//1 << i == pow(2, i)
			errorPos += (1 << i);
		}
	}

	numOfFoundErrors = (errorPos) ? 1 : 0;
	posOfError = errorPos - 1;

	if (static_cast<unsigned>(posOfError) >= message.size())
	{
		numOfFoundErrors = 2;
		return false;
	}

	message[errorPos] = !message[errorPos];

	return true;
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

	std::bitset<bitsInByte> bitset;
	int bitPos = 0;
	for (auto b : bits)
	{
		bitset[bitPos] = b;
		bitPos++;
		if (bitPos >= bitsInByte)
		{
			result.push_back(static_cast<const char>(bitset.to_ulong()));
			bitPos = 0;
			bitset.reset();
		}
	}

	if (bitPos > 0)
	{
		result.push_back(static_cast<const char>(bitset.to_ulong()));
	}

	return result;
}

std::ostream& ShowDequeBools(std::ostream& os, const std::deque<bool>& bits)
{
	for (auto bit : bits)
	{
		os << bit;
	}

	os << std::endl;
	return os;
}

bool CalculateControlBit(const std::deque<bool>& bits, const int calculatingLevel)
{
	auto res = false;
	const auto delta = 1 << calculatingLevel;

	for (auto i = 0U; i < bits.size(); ++i)
	{
		//if i need to calculate
		if (i & delta)
		{
			res ^= static_cast<bool>(bits[i]);
		}
	}

	return res;
}
