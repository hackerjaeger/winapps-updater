﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.2.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "977f996a9fde9701a471db8318eee1f01581274f7be4a99c7629df5ea62a9359455016d629209521169e364cfe8752eb514b46da4e659f61a4c2ca1fec67e829" },
                { "ar", "05767ee694be8954c1f814c268ce35ce9cb1c14f9cabc68dfca5e5b65d8319e831f71595107f5abbb98454530a91d852cb8fb943175c2d0e846505be8c6f13fb" },
                { "ast", "da88b2a5c84c74b1edd3cf274e1798e4bf2159b3d388ead7726be277b7ba23c785cddd839683eef016c135447f62c8f81bb4ad59d588c8abd078e1e4c9186574" },
                { "be", "58f939b4383b7cdd03822ef06ef8110c506059708870dfb55e94edb5484663e3cfac7272656149828b8420e5ee78bb96159f558b44caa9ec53e0675cb79c14c9" },
                { "bg", "1fd10a32037588896c07642ea29c4fe7921dbe8f8b4484317f73f07058aa680f5edd8dea95fe5a1f1297230d557602f025c6f5cf3d0021ad3f67a943ee67b00b" },
                { "br", "c927d06abbc7934056f3d67edf627376080cc2bd0c8c814805d5942cfb7821920c1bc5b7fc7c8e180e7e6c564291fad6db76b9332e2c5b208ce5ef1299e1cc24" },
                { "ca", "5f5889715c63365b25ef42218d1c5d08add005f14e1ee500b222f72ff7f85b68ef756bb4ce23756d8368ab4e1ac179f9965c6464e246051b4a0aa8b1b965ee9e" },
                { "cak", "845ca3a21c04a3ab3e82214e46d0dfe948b93e6a21a8dd48a4b8d6792e05dce9c9d7ad9f81342a754eccdc1194fe28254b498f0c6714858e7f8f5ebda4f72086" },
                { "cs", "fb13b3957a3bf677927e1ee0534efc9a99bc78cc2ec2ffaaf3b0296a6fb1c86bd718bdecc21999daf7545b22bf16ecdd89322e4064cd2879765aacc109af10e3" },
                { "cy", "19115778be6bb5222847c4cd5ac8f6653b47cdb31f068660b87d79322538a708b695159070e885f64a0eaf7698c21e54d14e537bb0a0f8cae4b625fd7e80904b" },
                { "da", "41c0086f70caa91416e2560267ae768181c9f65212be3f9b851d7fa951cdd463e4bb04c3321d3def5e4626ca79d69ef7544c8800313e6edef4e1541127abe9f0" },
                { "de", "3714f84213f3a0fdbea2264c7ff7e4e877fc2349c831586dc36012a40d3a0b451e0e8da6af2caf5cca83fff64f34f2c64acfc04b0cee3eb2f23c4f4f9035271b" },
                { "dsb", "2d003aa03d5475faa15483681df060d4284aba1f1b783fb814f2d9224b9a1d510a9abb85bead5f23c69ca302425c01a32819c8b6e93104a0a3969bc6dc62f9a3" },
                { "el", "3a9bd64a5a7c8dfdef7951c379591aa5ae2c4e0e89ca49588cb2415c1d4b5810ca4aab1b6234a3c2983c7fbb5a9fdcffbcad156211dba47e61a98718ab089ca6" },
                { "en-CA", "8f9069d50d0dd6f6ef8e21e69226decf83f64716534ce950e6a3eee763b5bf4c4c8015aeee1df3b836a8d5b422fdc467d37e126e3663122374f039ec1718ce1b" },
                { "en-GB", "38edf18c3d54df672991a7560d188de64cd0c87cee5248a0cc6bb72c7aa2cc038ccf7e0d33b163903072ebe16e6ad2c5781fa99aad00c3a5bc428190d3b7b291" },
                { "en-US", "a85243398de01265f771810545c3d31e33f4ccbd3dfa7782043ead1390e5c40607cd41fda79de0bc51326eef0049d8e744875a9c9ebe2e13028b7d136483c5ca" },
                { "es-AR", "16fd40d3d399916b4044627490364fd08d1959b54f5a576ace9b1fc3e96fb36c069594b2f13afc972e37ab910cbf2f98b6855d76c96000742cdb3f2cb1521253" },
                { "es-ES", "885dbcaee8340312f769b849802c4180135e4da039c4969fdad98bcd411a1ce4c0f46ddc47a496f18b7b78ebdeb5d25ae599366a7335c2d2c09b309bf51790bf" },
                { "es-MX", "c0574fbfcf05f84be96eadcec93783e632afd1c878879e77839db8e46773fa557568d06b07f0d660240cc8e6f4226a89e254efe4698ef8a8278a349d06dccb0c" },
                { "et", "7bd1e78f7384b12f5be19348c8fe236c80c77b01dc409fb1c55bd28483ebdb8f274af2efc80e10e8ec46797e2152c37deccc0b2f6f9e063d7496ca8f7bb21a5b" },
                { "eu", "0ac0b6f857a185dfd0a3e314ee4f5939c80627be11aa5e882cf1168a3d6daf16f051e85eec49a80f99a22d350810147d78df14bedfcaf489674d268d6a10951a" },
                { "fi", "2f950c57aadee959035b0bf22cdd98b592f3d90cd0bcd5a38151623e195132a4f9ff5836b42519e9d2718b6ff540b9bab8b60a115750ebd9f5a7f6bbfd1d91d0" },
                { "fr", "7434dc8284201aa5e87e1e9da9cd4fb97f1b22475626d8568622de3c0ce38e5a462c4d89dc6022b9ae67ca68336b24792880ff87091f8a2bac03fffb936f360c" },
                { "fy-NL", "b581afe4ddfe1a8c229ac3af4063d93ba548889791e3b33cb131acde3312cdb95742749af06e620437deed8710b43cbe74df3a074d7bdefebb1c03c7789ede86" },
                { "ga-IE", "3c5c3229481f8194f91899b9fae2c5546dab099306b6bc34e7c04eaa22b248fb4007f77d3c4af6175823a844f3884f4a634eed68977b05d5e1241df3b24ce6d9" },
                { "gd", "54d1a8e8cbc0052c398c17fe5da65be921a1407fd3bd149e02b6a1c5e889158ddad7d4fce0db30693a23276d7e6d17e9a4a8a1e1d2cfd6b101a76d9efe1b93dc" },
                { "gl", "8d6f316e9971af043864d9f9187bc10bddf3674578369623681a3e37a4dbc12d9134e8eb85c5625175456cdf961b5b4ea1f7a1aa35e396be3375c40518c11186" },
                { "he", "3e3f70879fb6970ed0bb7e8bfa387e11360d4edc288331800fa310d30a28f3f33f435e74858c155157e885b279357bb3f9d2807aebc1c2fb766edefd72a2baf4" },
                { "hr", "e149e70d473339c935d010e3226b2b53d050e135f0cc5c8fb9cbb3ba13d77bed14769205504377802b23de1a77edaca4324e94089fa8f5c7c2de842b780d864b" },
                { "hsb", "bc17f4684ef6f8f3ac41b5e819a236b68c8598eed7eafc5ba76f787a7b65e4e95d12d3a069a708e511931adb8719d4727dcc4657ed956ebe4188bf03697a9670" },
                { "hu", "15d4f7f3f42222a6cf10099f97907ebd84a6eae80e65b2b50975a3fe886f6a175b5576fd77ccb797c3309ec4d082e2b2900c3856b2eea21d0400c9ca38453fc2" },
                { "hy-AM", "f5e4d702ba0087e392c30078348724978e4116a2850599b2b81f1df7a4f88e923fca2f625f5a1f313fb52cb501b2cbe6e094be68ea812667c0cc5a983fbcc3d5" },
                { "id", "3ed7b700d346fd18492647409b91d8b793a884bc03f062eff3daf21962a7a2bf2b754d1568a85d9fb8b73b98098fd65070ac47650a675c0c4343a4cb70aa9bcb" },
                { "is", "2f1ea3aeb7a7c47a00c2c4c820cc95cc2ca20818c1b377f90cabd5e7e8382fd0892c3d6c4f496495afdf19f7c4c056ab482894281a9dee1f9d00e7a80f472d9a" },
                { "it", "af1678d37ba3ce0ac098db7d5a27a9cebea9fe9c77de3a993c54de2a2b59d2f6241a75e2894273d0a5503a3dd7619e8e8d5235a526104a58ce2398cf125e477e" },
                { "ja", "94393fe9e45656d552956f3cf330fa870371578ebadbe3664cd7b4a17bd576633944729799d077d417e771f80bcf6522c511d47c3542fc6347923eee09e6a254" },
                { "ka", "cc78e14a801cd0241f05114e1f0494f75a27b100d73c0ecefabfe4ce3eff0cadbdaa9ad59364bba747aa8c2efd5ca584169fa3b863b8e308e3275add75099f73" },
                { "kab", "6ec435f7771d41efc36f9d8fd6bfb3433a6d91920716e5bad5c058e7c031cf8dba47c170c76c82bd828e1e1f4ccc9aa3a4564e961bfb546513d1013abf581682" },
                { "kk", "fd3dfd025380cc18d78734ee470d6ecdd2049076dc667996c77ccf2ed21eee5f1f64eb46266414f6c82f31aca10d4c24328d104cad4c6cdad579cec82a80ce43" },
                { "ko", "8c15af3c0a5f421004f6ff559597b2dde20ca49e806b1b0e1a9ad7a9cb8e84235fc3fb2bf9e860775aeec6b34ef04aebca1cd9e51b76c98b51b07fed0cf1360d" },
                { "lt", "a3e3bc67ab92a57875c2d1c7efad256b362606341573158ade91cb1ee0b1be8ca887d6d2d07a1d29b04aa7e698be966f0de846208d523cb09394c9651a13a8a3" },
                { "lv", "6ae52c830a654bd44c764aaa99bdcd289d4a0440e11d3af1c409a70eea38758752e3cc03bd343bb80bb52c52b8cf7c514110a23000ebf0e0c00d9147c54add3a" },
                { "ms", "c8e33c4114edaa4b6653ba52fbbc4a30b4d1ba2f8f05d28feb0c67191e08bb3147270a06ff648b3ba5935ab952afc331b1cecc50031ebfa5bee1afaf93869537" },
                { "nb-NO", "6a4f8e8f15c951eb498d8c5d03068a053b8a82b26a73d89a6ca97b10381bef4cbc81de4573c74afb9a03c2899e733f22f2086db64d2eb284692ce38b848c60ce" },
                { "nl", "e9c06fd6b6db33803396c963dc4ca8c3b7ec17920f3c6d605098926698db9adba775d5a63724ee4f88904ac76ffc2207883a3760e9b6644f57634c5ba30089c0" },
                { "nn-NO", "cef8cdf52bdc8296974b42d105d1986a4db98f4a2a8637a3e25f69a81506bac83f6f4a44527244c8a0d5190a5fe5cc4e6deb05a79d437abe7add47822beacb51" },
                { "pa-IN", "fcc05eeaa262c4d0bb69d0a1579f5a7d8b1d707b03c0aad95f4fb4770f6deb882986e29a90565a9b6eaf2516e6c29f38591bbf803d4300c04109c3627dee839c" },
                { "pl", "e944dc3f0c756bc1161938230f89f7a6fac54e6691d9a2a56a6ec305e97ba99c4693a70faed7fbb5e8e851f8b5ec57800a989a4818a5ce5f783a870f1fcfe01a" },
                { "pt-BR", "b45acd8a7f284f1cd6ffff81614a6556fa9101b461d449b350a13fcf1074ac928dd1456eef0aeb658043b8990c553a25648694bea00027240e16bf820fb274b1" },
                { "pt-PT", "f0ffab17cee2d872ada9afc7f29846b86b5df7731d8b1179a8c1bae2096197e620fa16c72aaeda33f457d418f7e35bd4dd2b6822883cba8091226cae03a6e5a4" },
                { "rm", "0ec448b861b15b3982ff7f4dc433226c10cb91e273d1263d9f535ee14e22868e52b9b83e668f435d4f067804fa2165a6a439b911b20aa9ad15a5512fbdc6c016" },
                { "ro", "a10b6580470c8982544a4a7b202d199e1ef1d280a3db3ea705e82379d19ac4e455ea60a6a1f85f7f25f7b0291ddc7553334bc9e374a0ae99a8e6f7dc5bf8d129" },
                { "ru", "e0cabc57c605a997a17a44d61ac75459d6d61e47fa94f5a139a53bb00f1430cc9049741dc206c97e63487b9c469a8c4f944c637b35787ae8cc39a78bcabd5b52" },
                { "sk", "493ea8ba9be65dcfc396fd3075af70a16934f51458861b20416ce020a56215b1f9ff1ed174e4879f3804f89374785af2c7827391905eec38475638c02d86dd9f" },
                { "sl", "07ad3c8e5e4310897ec734f4ef75648325043743c2f67966020ffa2ff7def8c008fe53a9becc54621cff85f8993dd0f215befccafcb90111975c250afbaec2ca" },
                { "sq", "209b4acb50ba04fcb9ce503daee234586bc2bf953ebb5c476871e66bdb3489014e636bfeea45bb1405962abaa0cb988c601f55b5ac56a6088869730b330fadbb" },
                { "sr", "417f559ffc75a0b7cd27710b9d632d554fd045818ee64de94ce706681e2db13044edf4826efcf87a1be1cb4be58b5c77dabf63f7662921c0c2a8e9b9b769dfc5" },
                { "sv-SE", "7cd233ac82d351ff3635ae4af3e47cd40d510bf35c56905db11675a59f3d1dd780ab53f0bc1ff145e7887e5df2c2379bad100900d2754262e5585aed8397471d" },
                { "th", "8df250a264b0712847086f40fffa3aecac7eb88dc0107588c73afa3b66fe72a9c0cc4a7c270b50bd878e82e33ef7259bb2e055af7d00bf515ed9bb53f1dfd344" },
                { "tr", "ae504fdf64878e73f06d3a6e200f0b20ddba39bb9e9eb2abb7e27473bafca31f38914ca278301e1621d767e24e89bd5253d3099c58b0ab453e0601b2805df894" },
                { "uk", "8853a2c2df49851a52264967d4e597d8a38cefdd521ea0c6f5614743f46a94da1996dd52cb02b43f4f7e7c40039fcad3216ed11276113db33237cec21d3c9292" },
                { "uz", "5c7f5a131753aae795cb9470525da669c00699451fe2f2ae5de26579092e1096e48b3b332f9cfaf54bc56b7b6b4483f7cf335e4f7d65e4c6cdd95158557312bd" },
                { "vi", "58f200b50c8ab80e11ff74c2356c6542395284776981bb7e40ee9d5e6f72ae423bebd17284bf5edb9c7761fb5b17a96b2f9df8af8b1956569403f611695403c9" },
                { "zh-CN", "cecbc50cc4fd9ebd3c6e3b48fa2c8e3279d5ccfb67893dd8a6473a0d178d02c02eb972713a5c10ff887304db5bababbb97c643c34de58072a8f0c0cb5a91b6e7" },
                { "zh-TW", "fc4ddf4866d30e84373e1880fe6e09e3699f7c6912f99159d1cb24da3ca36d1e7836cf433d87128d6e1df5dbbf47fb7a46de9fc383fcdca215320b93814802cb" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.2.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "8ecb7338d24c1d010d50315220bca386ab5a3ec7f0da366de3eda4421b77fcf63bf1ae9b29aff72c07f367c23ad93303111d1850e65b93678a9b52056bfb79a6" },
                { "ar", "91ce687b0badc8f89300d235d3772d85cda5650b25801d3c5bef23a9ec2c871d723f3dee60d1ad586f420ad581b8d8a12750b73820ed73bcf8ef5084d3d433a2" },
                { "ast", "f54e5f9c74faf7ffe5f58871ad24c542f2fe567b65bba1a18c6e83618b0f1614f846ea3c01459d2664480c90ae108e0cd71a786d0c0ca454cd600833ef542a54" },
                { "be", "c7cf6e3f7ea56112671e28b4ecc5caa8ece2ce46ddf42d0696fb9dddda8321ac516f158e0dc1d4d91119b6df9727b37c4ddd8ee9555dfdd11762472ad2d8cb89" },
                { "bg", "0dc1984ea83922d6d9afdf5909ca8ef51a0dc4f833c18b0a152533fdebc56d485e68ecfbb121caea34ad933448fa88d4212efa9b35712dbe327cccf81f6094a1" },
                { "br", "950d904c02628a35e592ca7f04707b6a67cf9dae9184306ab1cb611c5d060840456c3799cf86d045946741a09b47a1199d8b3dd9fc60b615cbf0098253ff6f49" },
                { "ca", "65b84ae6b867cd46fc672217c27b3aec6f863c86dcd0a4850bc809c77bbc972395f9fa6466dd49f8307f8ed4c3627eb0c6a95a1bed5c3a8bcbcaa5ed14878395" },
                { "cak", "cc4d48d451a66b767f9c56a9095d064791833a9dc996efc5f19048555d38be915a241e1508a7006240a3833e57ba85451eac43b9917c153e32792c021d26187c" },
                { "cs", "8e79019733782629655ee44dbfc7605d1e9a5f741149a88478ed332b3e66e0612f0170fd7b37551e2bb8d8d769cb6ed9dd5f5171446c9b7248be8de274f216d7" },
                { "cy", "2b631ce53221216bbdbe1a38cbb12d0094f106fbead7696628e959f72308ff02eb459da38109ec710a62d88bfc65b2949e25fc694e88075317bbcb7b514ad68b" },
                { "da", "bf690783b4a67b1464850cd90e5c34acd1b0071001517397f2c7d6b05c36558df342a49a07914c69ab3145ceac8d14eb6c4245f4bf35e52bccd59967434b3622" },
                { "de", "8959afbafb9864d06400c03be986337d447acadd3953228491c2c784f9ee0b61052d60c278712b26f54e951fc1275694faec1da45737037f05cba8575283a4e4" },
                { "dsb", "556fccdfa9e244d28ded723ab01e6b0e4e3c484265573a58a083e8fee541dcad7873bb41727e7d1d7f2364576e35ce1b422db53bbc37568ff7e3e6229a8c4eb3" },
                { "el", "a313a0136743b974805d63055a63da9e41c88168827c70c217453c45187f5137c4f8f4e97e91642af7c47e4b69da931a57e8afeb378eb7e523a37f0fd509e7e5" },
                { "en-CA", "e648824460321dd87d7f9f59ace7c06f5a9adb19d809003179f231381e37bdc686370856a8d3542dfa3d0134c782e2c2dadc6cab42e4156749f6010938346bd0" },
                { "en-GB", "9d3cb247522ad97b0299636edd104e3a7145e7b4547024b7b4da72312f05174ff9d746e5dd2795d661e68e649ca6d58d146e6cb8d4d5ca8e2c673bdc0541f2f4" },
                { "en-US", "369c60ac5b76ff043262fe38502f3b34155f6c094975ad11bb7b4e50b2b77b8850602463fce8dd7d296527f74c344293471419207b385cd65b06e850eb10234f" },
                { "es-AR", "8b65c0dc805ec31e2ee2c6e3f3775c776e666fa37fc90b8c55010df66de12e24a92af7fb447a3c2b4f6aa46a89b0d535f0f69646b7ca0813e4ccac99b54b4054" },
                { "es-ES", "02e9a540ee62d28a58fb122450ce25a376c2bcfc4fbece8a585e5784e81527f43d8b381624dabd18b6bf3be35663ddde913e2c488c5b28d4f20221220e07964c" },
                { "es-MX", "3c6a116862c6cc32e61f03e2794f3d17d37625d5a8c0206cbb584103f4cfc96408933f3c5fe89f2819035df8fc4bf65efb5128cb05c48a859bd15ea112c04cd0" },
                { "et", "3556d49800a4c5ff7d1bb15c1ac93071ed5b73d2ff100c7a7d692f672644dd9115ba42643e4d7a5daa834a225be28aa2733050261262738d052e45d0cc59f0aa" },
                { "eu", "0cad66de4109273567b48f9e92855af4df95d2a8f630e108e2ec8a58a9425d5fe43656d01132a2d26a3a9302cb3c2caa2d4c2deab401ee5aaec2ee01e7480c4b" },
                { "fi", "9359094e6844ef382827572ba1b2a085d3428f41d317ebe8c193b9116156d32a189c0be28bd9978bf096e8e21c8de5e4dbc214da3186cde252edd73161db0a93" },
                { "fr", "6a3ffcf252655e52325f3543c1db867e1dc12871efbccee85c8760fc8e90603ad785974586500e812d95afc3e0d427453ea60ee34f6f7252185f22da899d38b3" },
                { "fy-NL", "28dbcabc7281c893bfd9316894872c20ddfc943f279f7238cd40161d1e127f1ef34852da04b793eaa757973bc50190925aaaba75f55c00fcae44311836c87ba2" },
                { "ga-IE", "385b8350f0306414203c0b9d999695ba603304bb9faa7cc97b53499c73e0ac46730bfdfd00a553ab1a82c1e558cc233feb2a85c2248d0b3821166783be11fa2a" },
                { "gd", "a24ec1956c2223e169e24a26d7b70700af9cc5758d3c0dac41f054c9c64e00bfbf215ea32f43e494191343c9fc895457e316f56ebe0a89e9bc67e0cfcf57fe4c" },
                { "gl", "8152a4a680cebdfa9ede29cf2a031689caa78b768db465f861ce89b3c71ac30239019e154dc4bd51eb885dbbf27b941c94b12d5ddabd389e73299a5f6bcd8add" },
                { "he", "7a07063101a58c2a81160a79b42dbe151df651399eea16f01dd876c096b25a3bd2be708ea3c8a39180bcb1d814a0bdab2c3273204862d7d1f1121c66324e17a3" },
                { "hr", "885372ecb8ca862ca9da59982795a872f8dfe84e9a6ef848afc5b320c36f55bca1bd19648cb379bedf53bbe2ecda8c6a2a0535f2180b565b4798f80938ae7563" },
                { "hsb", "7af7415f3055c8e7bbb23f09bd413f0d964d2bb29de062fa3d010d7cf7b05d0df360437e4e0bb3d33932949c9507ace4f5db65418891a9504af8bb1914d2e486" },
                { "hu", "f440e63661883462e36dae6ca6987bbb3bbf0eae5995e2dd254123936589ae1cef548bf2cb4c619fff03ad1808915ee224344bd84bb6b1ef4753758241702c8f" },
                { "hy-AM", "952853ffe4a8095290055bfdaa57e82351a05a73095246791a539feb72de485d2fbec1d5795655bfa8ce09224fc5743206defc77554dc2b576c35672297b85a7" },
                { "id", "c121c089e1e8dc624108e099f4c2cb06fa7006de74fdbea2379eb1757cccbc67ccab297e06b642527ae5ab8ef9b92adc765859d9d4ac566c2f9fe1948e8639e3" },
                { "is", "8867f77e0de4d8c1c53520a61620970fbc493a2463f806d516f0300f833fa1055a4ac3c6c8c639e4c05f1106d20362f6f967e64a9391e4a8b696a5cb5d4e0bd5" },
                { "it", "37de0567ff4103872f5fa1af270b5f20cfee6155a2e0eb0873cf563604162cfd0b2a1758da84ced53854b1e6439a740ff8ba723c4b99c278cd7632e47f2496cd" },
                { "ja", "6d35ba96dfba7bd396ccfd6bbf740897ec9c729fa721b87cffc02f6cc7ebb402dfae56346a1e9042fcdc1987b266fcea85255919b738088dd690fdf2a0244892" },
                { "ka", "67e1ee06472255d74fcb37aa754be21629772d626c6cadaf0cfa9db639b9da6a830cd27e2fd12e197d25638e786c47b5cc1d4676b12f1c3913693a1a90d070b1" },
                { "kab", "7c5da18fff90c1f3b385cda5f71a8290899fdc80bbcc96ece9367b2532ced6991e736e27c1f97a1b3cf3794fbb3830806bb76c77418ef8325e17a6ca2ed18f50" },
                { "kk", "19dc04ae8f77382d453b884c295304860bacb0bf54df5caace5cab3be8e9b48821757b76dda701fd2e1dd8b7db23a692a846597c3d727e575ab29bcf9f3ef514" },
                { "ko", "8d8b7dbe6954d080fac998a869941ecae1df837585237579f43d03088bdf33ff102a8a2b250aac3e975ac1ebaac056029519475e2cd1f1e4aef8288819806b62" },
                { "lt", "04ca597b3ce30fed93fcc392cf7de74aba4a8b17e7bad061df6784ff7ca0efb06725496de704c8fb0e499e9d2591a62a0a5f293865f24e56306d2b70c6c17e99" },
                { "lv", "1a40feb4fa4e64bcce974bba2b0d57684c6201c834ad883fe01611d7117f0ce3a708c34babb76435df189446b97357792e96ea37f873c9db7434f59c8a4be47d" },
                { "ms", "fdb4c13b0821d79a4b00018ced9c3d3bfcab32605617fc63c07c67bde6c8d04f02abb2a5cf101bb614fdeedfea2dd35839972f7251a816ee8c9a1f8f36a73f15" },
                { "nb-NO", "274b6a7b5eee4380158e8f6a44632504f5df4bc76ab532fb6c06f6c3fcfeb10d126a17532d59efef785ebb8d9cd30ff8c536b5116e374b3c533a4b984ee92822" },
                { "nl", "32c63a3af1313e0d458e910b2a8b181c93e0c5c29792f78bd2ffe95e2dcb965e40b68f22351808e5f45a3f9baede311ca3d192a00a5115da71d328e2019103b3" },
                { "nn-NO", "b2840acb34e434308f166c35f9952a9818178c848553ecf0321051ab045b4088693e05bf3fb8cfe0df1184ef7a5be8447ecdcd3e89c9a8436be49128f465c1d9" },
                { "pa-IN", "368eda56bea98fcafb32cbafb3f2cd154baf8ddb25fc6bc716f733bdc9b77485e885beff2b72ee322ef082e5114a621280271bcdea9d17f902fd06589fb040ea" },
                { "pl", "7013c618ce7b5c4343968f75df5075b1b817e008c194957604ab0371f88e349619b22e60651ba1697e2f9007ee9dd01a5a1e0e4dd04e86ab38f8cd8aa295c4a0" },
                { "pt-BR", "74bbe0bbc721b3b8a2b82cf3fe2193da2f526b109d5df36e836f0740830fabafb52c781fbb9574007598a95a1067729589fff2b85e90468c49ea3d6b46b66ec2" },
                { "pt-PT", "5e1918e4af576f903bccf514f652f006023ff1ac27e9a3c63cced7c21b5642a2054c396b709d49a86c3dd8339598427da086adef998970975fc08040f96ca54e" },
                { "rm", "2be468afff72eba22d1e35090408f516f211965ca6f589ef4762a60e95f974cb21d440c9fe4858dda68ba529ba614dc4af930842a507163c045f78948194fc52" },
                { "ro", "1dd7c7f4b5fa3bbf5f326edd08bbb5d6f1b8aceb01250a7657665026fd0683781754cc376356e41fd8655bd720fbc3d3e43a22a2ee1d35f2bd1123dcd1b6d3ef" },
                { "ru", "29fd0864e3deaad2baaa7eb7195d7e9e4c05fb49b6017e6af7dbc282d99e155e4cce3efac582313086fa1f98201e930156895a48d2342ecd1bbb00d6068432f7" },
                { "sk", "bf83a4bea3a3b7d51da229e1fafc3060a7494fd35b5e6364fb57b8f0ac82fdfd83068e2e8d13d842cfbb5248cd599f2d9a80f52acfffc269d30ad21976ac8103" },
                { "sl", "130838a05ee1ad006b7e42ff034523ba372ad80ca08927417965438c593058de894b926769b2e90a752c9d5d960c8163cefe399d4779dcdd78f597df291d870a" },
                { "sq", "3f1e98b813731331b3f8b36be2ebc41c191b1cd33ae1272d96a7775ace5d8a726858ce638132533a85a793a3364e6c8c5693767ffb043c4146b9d1bcf1f4d037" },
                { "sr", "d25b92b984c466de136a579939e5ff61b9276103e1e4bfeb041a759c310d0ea90a92c7bc5ca6973160104cf4a9e4498f48ab067308439673f2b59b41f7e9a9a6" },
                { "sv-SE", "72cd1ca69f365df52e7933ca28a0c82f4cf28b1f38fd8b62bc2a666afe1727ba3478adb325e20d9bd892300c3e1660c007fc3eceb35a2a500173d2dcc58a129d" },
                { "th", "bd754cfc8fbc933f50b2d329c2ae98d9cd14ce16c07e6fc3e359cbd3b46af2cc98af12b453fd1ee932cbd79305b4ab4422c6e28e0e4ffee846985485c843c7e0" },
                { "tr", "2a4e043895bd725bc4348b9ecfd4e1ccf9948d9920d5b4bf59ada520df18c10997b26b1909da8e80385ff7236ca9460118a138a9ce0ccbf2af22391d8fa3ec1d" },
                { "uk", "207f909f73754cdb0875116b0b7aee45a895d5176afad4c1f9ec9a96de6ae55e4b4139ed0866ffc98a3981bde062ad4cd40efa5c9b5bdc0335089f0920d2863f" },
                { "uz", "030167a193273bec24e77ff5832a7bb7776bdf92ea56a64d48c1efcdbe03319f4704d9fa8bd11d61551b2c6abd0c7a7a1aa67c1c33e9e0a244595a2b3feaab97" },
                { "vi", "e9b167ba5fdc2fbecb3dba80ea013020ae427456b2aeedbda549060c4839f00cd83b56d7b2a0b12f4804b8d0b495006326d4f9849cb4866a9a1c68d7dc23d814" },
                { "zh-CN", "a2a5c7003a7a0feebd31719329df0093693f5d6b4ea0df0242afe6b1fd28ab75936312d32835d6ec1a05bc60ca42d048fe5fb1a32373dece3d847c6e047386c6" },
                { "zh-TW", "6785466f3bfdfd3f7d8ada7878ba70e891279964fc0bb2068908a3115c6fbf28da4cdb7e75a55362fb4cd149956a8cea329b0bb63d66eb299c8bc6273a5e3db9" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "102.2.2";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
