﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "115.0b6";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b6/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "e1590bb1beee88c21d2c217b2c11142c8dbe73b5bc2df8ec6d2e105c1b93f8dc14325cdc95b5fb0251b8983d8653a8394e9038757e175254a507fb44578234ac" },
                { "af", "56d051445fa449ad4a4e9b3bab76ce699d1e05b099b74bbe81a6b26b8b274499b291bd2a6a40f52d92a23e09a7f336eb85eb7ce1351ba4adc21cdd3687acca7f" },
                { "an", "13724849c3ca80efa825f4a3f6c76964b585fe0e8f1bc05128815076a35c9c2c647f4ce943691ca94ec4f8aff55531819fff3d8661b80eef1e03800a08d0aa60" },
                { "ar", "dc45c7845539cbac9f2c3d17ed50b8f42e061dde05d1577e5173dc0982bf9174620d143b1805587f6930e68255bcb5aa919907fa41b7a0737e23b6c869c86ca3" },
                { "ast", "492263dfe9579b5dc3da62ecfd30e82ece3c16139eed9f27699eeabaa2b4783adcd28bbe8af433c441a5e94753bd4b62373e4094a4bd8813d9b1c99c869a2f3c" },
                { "az", "0c81825d309264ab995067c4023ba301c83630fb3b6a19a89e22cec4c1d82a36a011c4ea27560d83131f986c99161f7bbb16963c8e1f19adb883f51e6c26e7d2" },
                { "be", "bd2e333009d225bcc8046af40af833d5b23d09eeb4e3d63e80d6a1d6e1e604ddc4cc1ee60a1a111e4e1cbbdd528492ea5efcc9def392b202078f2caf1f39334b" },
                { "bg", "6e76532bede243786642a599db6f665a04decb6f6a79dfea3d876cbadf4b46178566644d7c4a442fd1af2f6920714e43915a05e856701e375f998d8a8604cfb9" },
                { "bn", "5521066e0036657f64a0aef1ce279d314e7a389f7a2fa2161d3ce66352b8d6afdb2aad16a4fca499ba29fa7d69162805fcfbf0a995ac22f313102ac401411412" },
                { "br", "3e7626f12b4d30efc3e21c79f74cf20e90ad7ffac2a6520dcea608c62998fca2012588d575294a23016958985fbdec2c59f8b84d22b6e2656d80b24baaed2619" },
                { "bs", "c8d6360d7dc5c42f237df01440d635995fe714e0d7e458b3a54b2a3da9aabefd10109d55a57c610f301d95398f5c6d40eb7e3c5303946e505d58212ffd0a0af4" },
                { "ca", "4d1890a23bcfe12548d0b7b111430e810a73a9d4934e51352d23f821540fc497c7635122780bf967a7746832d67475237fc3a1c23a7d53c413e7d0574fe7aa25" },
                { "cak", "a154752c1af042bad669bc1cf7bb4ab15771a1a5c682bf5ad0b14e501fcbc498fd619ca466acec75605cd223c5ad7013195b8ac7c9e80082e8a920e8701ff356" },
                { "cs", "2c5199cd1302c99aa2ff982651c416cfe030602e96cb931ab1eec37b9e19b7079f2507264056cd8c51e4cd4fea662e63cddf8c9b71d6913b402fa0da588d9f4c" },
                { "cy", "bc96bbebb22f8d79f4eb2d9e88b170896a966c2fae775c1ca1982700139074281180661ff7cf89234a7056eb473e253e5a2bcd5d28bae3e80275c157e5f370f8" },
                { "da", "4d1b7077d3067a5d8a901205416d4b099ac14a8b54129f938dcf3dd76688e20241b684b287375281c957390ff188f5bc3528ff91d920d94627c2bbbaaeca072b" },
                { "de", "91d35d94185daf15b0066be0d4034c0cf61856912f19cab0cd566dc843d60dc13a8fb1262d44a9703f8f39a2a40c9ba07d16a3a07b4b7ed69e5c225bbb5e0dd9" },
                { "dsb", "550c40acdba494e2fc23165f49fce912f1e8931b65facaa2afe24ebbd8dac30557da3d8c37627c1ca06dd9b31f3bb754ec3e2a437406a11d6f496f7093912874" },
                { "el", "a4fbc72483ce0f9b9cd2f522052717f5870222fa3082bbc7868caf6c42d86ad7f6cac459e936f95d4b4b1fb5ef02f851ed3f3a6c6aa151e8e06f3f6c4ccd7eee" },
                { "en-CA", "4f4369f428e8e737b1e868ae84807410fcf3540d5eced50e47e06c5c62ca0d2c98aad706b4bd4d0ea7d0dd13fc11282ea290e52082489dbd5901b636fb88ff56" },
                { "en-GB", "0aca27edffd09eb32c295c55277a00b9370097f0fb7227299c932f0a1be4738cd2cb2bd579664e1c4c5ca212b98cbb69b55bba752c692033342f8dcc9e151c5a" },
                { "en-US", "a87bff92b09eeb392fd82abca0b7f259c65a81d0bfe95816f82227fc6fee4eba3084447e14b5f36b95c1e5b1f0d22587938c4601174ed41d512edb08a928ed5a" },
                { "eo", "c1e117bdd7b3b904861d30e0f9cc55c200259a1d74acce0e91f915bcbab8657b3a5c4b222f9b10e14831d9e51ab26191025bf17f873b770570d93761bfe81bbe" },
                { "es-AR", "b52414a7c54dd3eac20ebeacae423edc9c9597e3ca3a30227a055a953458c363c3990ddb043547f3f16a5278118abdb522055e6baccbfdb0ff86e530bdcc8f9d" },
                { "es-CL", "7e0a2922600a2b97b8842efae36985807c75094ed645c49bb09ac6262c929f69f167846b0c28c1e86afb35f4d309e4c443503f09e0cebad38ae012197feeb69a" },
                { "es-ES", "c244547d17f195585535fe5970e7fb089e6400c4c15a0e0a54cf5ce84715e9682f016735b9d3b343915821c3ed1aac275e8fb05b7b60dc468fe2311bed6d6a17" },
                { "es-MX", "a16c5ab458dcd2cbf40aab1ee592da466c14d7a6ffeeb1ac01c4cba434205d146da81fdf25fecfaf0c11bf286d2243d62c2781c48265690e66d1f1522d604b9f" },
                { "et", "d1591ff53ec6402a62b84dfca0b04cb0863284a554a6233f398818a38ab6a3fa78ffd3c010522fdcb460d7ebaba3bbf1b0ae979568fa74983505aa479366a175" },
                { "eu", "479e418d7ae83b083e7205e983c51745498c654f19e37557229a64da9b411f16302112fce30fa216b37e88d3ecda21ef790bfcae252f6c151a602b0a5e482ca5" },
                { "fa", "cc6008c3541e5852c5b122c15aa7fb59fdb49e90cd88e138f56314101c06682d7ca095ba02136b5e12917ec95568f5c6e260e20323595a564fc9e5d3b2c44ec4" },
                { "ff", "249592808ce84f8a68a7dabba108dec20c608e16c9affd7a6a4acaed676ce29cf57445ae71ded56e369a85c79c28baf1952d47729106140e93bff0d52ad5be57" },
                { "fi", "05462982d26409d8ec270d10e89dc8d25bab79428c5181c10f5e6e4f9d33e6cc3c72af965935c031349f12c537f76d4284233de000fca89e341f10821131fb75" },
                { "fr", "8b7cd1ed54206a6162580986991a427e008c60a2bffcdace1065545bc024a01ac81ab076a924a7a2a15016a4231d6971262800521c22f3d7f839f37d1166c4b9" },
                { "fur", "5f92e8a00b442cd8e21c005699295ca1a263c730a1abf22722af42b47b71efa533ad0bc6e1229f5c5734435f5a7cbb98c5bb5477af9c54b0b0470eca696536fc" },
                { "fy-NL", "0300df9d5ab1073924a1ae2d4d324897ee2dd3bb3bdcc33509199053fef1540d6095153203ce504468b99627f744f35c9dab6fedad90c68eff466fc435912e74" },
                { "ga-IE", "a973e923ab3b5a9124d46840c372fe9eb00d0ad0d418a8736fe18eb563bd928e23489d8ba6a1358009fd66c6efdd01c3940a623e5dfd03d05470fdef2a92621b" },
                { "gd", "7f8cbfe491fd06364af2db16d3dafa35c6b8d1dc5121fffe91ac42630375b7d95840523c1bc97f200f69b7fdd3622258327f2bb5d5efea431457a12f837779b7" },
                { "gl", "7be6cf6915eb82ca381a79d95339f60a63220d875249ca7bf51d3a05f7cfa8b98db1aef6bdf6576ce837b213661607c8236f6d9eda3986107538d742ff522862" },
                { "gn", "0fcad8115c430374ea395900817942d9aa443b45e1bd16fadfe6ea11111e0ddcd2d450c62060985791f1c6b54fd6aa58bc3ad6e4ab7626c6f6a1b60e78f02169" },
                { "gu-IN", "a2861f35fe40b246e34b60787fbc7763bf5d97b8afce3b617c2219e6e35172216dcd25b27d2ba561e3116ee8317080fc8ff0848bff8c965cb7d5d535792ec0d5" },
                { "he", "bcf59f38046a478cc54da0bcabb9fc4a583cc75ed5c10419e5060a7dbda0284950373c5576107c4d3de07d7bc4c164d25ef3229df1144486b58afe1d04dd44d9" },
                { "hi-IN", "96d816f443ec19b201616609084ab8aef808d7c5d16fccb22e57e7f51665ef2a5da5e378d3bfef89b19d6f0c2cbe106b4facdd149fbaf0bd8fb45f5a44b1f0f9" },
                { "hr", "270b75546d6c2bd6c58c310db669e4d80de5a9d781f7cfffd2bd176c706bbfff380f0d884317f59db8d0ab021873b30eb1dc05a33563aaa2931c57bc56991413" },
                { "hsb", "8827c11502736aac5b510f954934e6680c04962269225e849754a5d63cf19d4a623f1da25323bcb8abd7e88895b661d89d4474067724fe0974286fb413077680" },
                { "hu", "6d6a1a8f5c96c0bb60a11f4dd9859880ec11244523f68e87bb15b5a969b077b7bd9256558f473ac0849c1b50c87b73bb2837edc857f533b683c8a7a7f174ef12" },
                { "hy-AM", "876213154357f143bbbb94e98351dbba216d843a4d80e78d74e43c11d5cfc5c0e90a1443e9bf9466dc75f2569968c37d56bfe31f00b20e6552eedcc7254eb5e4" },
                { "ia", "b3a3d7eccd0f45d4ccf1970fdb2c847592450c784c90fe6425f663b3dbcbb829dc57db9fc01f2ce2df2e3bf1e5aab7573c5c771d57e6b96685fc8ce9c7d291c4" },
                { "id", "930de94c5a51c4bfe67e300f7fd9ce38e7160bde09c09a054803d30e91dbde0341f2b18c31640bbc52327d726405cc64f1b3cd7b5fbcf8058d216cc26e8dbde0" },
                { "is", "2e67c19219cf8ed372ed697eb2537bd5abbeba70b2b1400183c34fbd09c9f4bd201912583fd98af7fd2c23b402b9bbeb24e4ddccfd5805ea49342def52dc741e" },
                { "it", "f9e2daa6aaa6b65f707de793d79a2a3b4aca23985b204c4efaba8f0dc459203f780e7b8e47f3d10220f37b444c4ac1c050bb2d3bc810cb26dcbeabe932b4bab8" },
                { "ja", "611d4006a89cc1a9f10aedd9981e3fad863f0f515fdcb8da83da4c2878419e8bed959f285caec2cef91a9598b071718997fcc709688b8d2a1225160c9f663a5a" },
                { "ka", "162b0fecf625aa0b09a32af7f4a1e34ba603644004bca012895e804e19ebadd588f50e783a62414bb32493a85da200ab5f8d583756e4d7644aada59b645b5285" },
                { "kab", "aee3da7f0f9a4b0035d379cadbeeb4e29f9087e84934d04ec8cd65a5715893c69996dfe75895d0ff3958ab671de6ab2b08d499134b56f995565aed5f97ad4386" },
                { "kk", "aebe284d0be9f2cf7b6b0fb4efd349daaa41d68c8fc018a01b96017ab9741b550222b85c81cfc502fb6cae219df106cc20193e6bb2afb32243e2736cedf73db1" },
                { "km", "d1e47a3ae3a2b6fd18f923848a45328fee3cdfd2194ec602be562256a66b24cc47b6dbed9a0fb16dffbbf32a1988d1b86f85d246d0658c6c6a7b7fed9b5b8863" },
                { "kn", "6153ad1b5c64190130f80fb6be3d31a6ec19bb9a36153fb7358323bad3240f28dbb95c06eaf9d77d724ab5f2daa66e8019502fe8875bbb422402473c03bf71e8" },
                { "ko", "e806dddf7fb435d8a33d6e9a02930c2dba2657b8d5c293859c5f62a88cab1945dc02c28cce4cfedc37bb08b3b2a5fa1d18229b8dea81eb01a81e2e27851e6d87" },
                { "lij", "55fa0a9da7d0580240ba4c44da7a06808778f2bd94323284f1828ee8b5c420d5d61da70f5950c0b8a4a4724e2981544eecad545c6604a484643ec44ce6bfa454" },
                { "lt", "e27b324a9a3955fe8cc30220bde1f9e4c5f36cf6be8e268d7cb8c312fa33d8ffd1f7a0246359219a05a5faa9785f8e7f9bede54dda772c00beae69bc66366d34" },
                { "lv", "ac940ea473169f2ab57f79f4f95dafb5cc7852517c70a4035f1a51dbb9e5228d637eb0822d35e38de891d7017b87e6b902f15bcac6a895135019ca53dc5793a0" },
                { "mk", "a7e54363f7c06476f20e86cd7268d641fa8aeb8650bc42931990a07aa1d19eafe5de14f95975d18f49ee30e79187fda4e315696f60440b5ebc698745a67a8551" },
                { "mr", "1466364785830395ddc1fc5b5f996c07a92668ea338343d9637dda49ceb6a5d99bb4e6e9a29fceec635d08b7a4df6aeab3359152802b6b7b51d248424e813c90" },
                { "ms", "cbfe735fc4203cb1042cc8074d8325c2bdda7cf14d436f391d5b03305c41a76aa955e50b960518ff87b4f6eb1170505768b7e90172f339fcb65d75674ce8abf8" },
                { "my", "9eb89d20014db1f0b3ad9c99d1ffbbef7d2f8f256e3d11155cf9b28c66ceda711f3523532e580d4f8676dafe9528192e5bf48832d543a4f299ef8789a37d7588" },
                { "nb-NO", "97727dfb93b873c0b6e76c335e9532e6216072f0f5874d11ddddfe52ddb10dba5b4bd4114934bce5c79109e1d095533239bd20c7a751b10b6ce6334779fc56d3" },
                { "ne-NP", "7fe9d8e7f96df15200b88ef1c92f989494dc4af486db9249f843fc9f1bb5e23629b70239506a03b94285bdd47bd1c3331d63480a2d1423798da8de2012f794d6" },
                { "nl", "78a4d59fd67fe147a7a14d212a5178f6b16b92b218e8d2b8fbbba85137640042dd9a68da6f3d72feaa91ad38ada103b249fef5d244c9f2190bf013ed736d6d57" },
                { "nn-NO", "d075f9bc1bc11711ad08a52c2ea99429193d9bead20f9b88cd5de37ff7e14ac4f3230e4eece94b4848566f48f15e13e4d729a3a0b18274a2e20a49775fdc7583" },
                { "oc", "04c42907a3744c974fac8e6ebeb8197b1f01ef2ba38a85cb2ed73d1dce87c62d6983deeb5b1c7c55931cbd55e1b1f6df88f6c54a6114b9d1e1fb0841a61c90af" },
                { "pa-IN", "26636841206d87267b4f15e0fb8f766bcd701514fa9a73af17fa9da162ed1f208dc48c93241a6bf8a6fdb7ba485b9234a4e7279f1b6b3b0cecc0e367a16eb830" },
                { "pl", "6c430bdbd0897b5260b6f831eb746aefbe139ef0e40d54c7ee00f15f133723d2e7b7b12069e1b11b6d7ae5516f83982827033f7452313b648f0e0b971da6680f" },
                { "pt-BR", "faee5ba983c044eae9a0c3bf153adc7cbe2bb947745280b3b65ea36fc37702a1d029603300e7d08842b15855b4fe1be6377ca87934b06e9708939c707d7801fc" },
                { "pt-PT", "bb4581b587a6e476da6c2582e953de0653d249ea00e8259bf80fc2b3cdfe99b16477f5faebe03d822c593e545062245722233c11adfa6d905146bc3137392028" },
                { "rm", "c391cb3a91e7c1c6e6ae2251eecad190e32005decc44d6b467f0c13304deb42f81e78f63015ff066f6760150c1ff7e8cc23841228cdf952bbd130f6fb4c1a3cd" },
                { "ro", "1009446b453253c331876cc2c3708fe9acfa6526366d0bd4c37236e125a303df7512251b47a5b5326260e622aedb236068739f1067d7aaf6722307d0a0c6029a" },
                { "ru", "edec4afed3e3821479e91c459802b8a11aaf7c2e687df7f6fa5340cb49a69777b025db93075a62a5b792c01d6d7c1e5c38843e390ce25a3a9befb4cb1194c4d7" },
                { "sc", "38fbe177dcbc2a3fe6f1ece1ffd9cc7a3b0f9d0a18a56d0e22a1bbfd6413adb3b37684462c7adc4b33f12480104cb92966284c8f8d95eb5429471c3bbc65c0e2" },
                { "sco", "72a2ed6edfa82386a6192217390dff7beaa1f3b3cd753d84fc912d6def5ee0710eac5831665837f4bd4ef31b3add82d642291a7fb0f21e333a63e53097b72239" },
                { "si", "fb597c57c7486702ed85416d95074a9f41eda670461965ccf7c84cec0a26f06d608f1b61af39765ad4f1ef04be2f62c68154c886ae45ff72367d8eb4e26e1723" },
                { "sk", "d9bc657f6f388b633b53959279715f535897de633e9c5b8d5b64a29c58dc4812494615525d733ece4748b21632613bac9c22311648b833d760fce768937c97f7" },
                { "sl", "e9378d4d82947b863556fa0c837895d19112c39befb62f43d7bef3c20ee0c4f24cf34cceb7fe291e06afc8297d95176fcc180ed9482a68f47b77dd02e37b20ac" },
                { "son", "40a79f0fd7a334bf90969d1571f989da0aba41b1af3d986d8ef42192bc778aa4246f972332c66cb700c5d889056084b81ef4feecc5931cd94fbe61489b09da52" },
                { "sq", "0117225a7d21a473683420947120a63c6788b938833ffaef0748b5922c3b26f758d578a2ee929d61dfc892e7d3da4fba4d0d4d881c4dd2124334526fbc25a5c2" },
                { "sr", "4024b8ff3008cc184b5eb93c932f8220e65ba0bc67a0a53937a77ba7d42489e1af653b540604e28c2142b35c69450ca5a40c126ae6c96e5b8ef7c6d36f05155b" },
                { "sv-SE", "670c8efb9b5d3266b3e5755b468044b3ca800f322c6bbcf2ed90242797a0e32be03db83da1d580d4eb8aaeb927d1e9626a843668473eb9068458e547aaa1596a" },
                { "szl", "36744b04de9ca431f6120949169054dbaf47fe46c65928d5289613eff3c4c4063a018da122ca6a2c3a048f5d56d40c9cb152119052190d54bc9a834b7e968798" },
                { "ta", "1202531f259f43a66245471839ba7dadb3f3faf10e47b041136d3ae0d13d5d47fdaecccf81fe01953d7587809a5af9e73644c81ffec33ebee31ac9f53b178cbc" },
                { "te", "01628983ed3d27a4db7992aa4cfe9fa0be0b24e2e5d32dea1b47e4db980533963efe91de1b8b5bc5819b885404a96df88f4e30030766c394d55556b872b3c369" },
                { "tg", "eea0eb09dbe23cabe75180c2a4317b38b7ad38f8dfb12d08a9c2b8c31a2ae8be932e0646a52286fbf9f38b94cdcfbad37981f9eeb1bf175eaed27301f00bc96e" },
                { "th", "67e0af3044c7a78759e2e9f69925c2c51eff3efcc760a5c596ab088134aca89069cf97fba8e920cd9d402fc92a1defeebb76f2aa1a6cfd56c1882a2bf8e860a8" },
                { "tl", "aae793573c152bd0089d75abedcef9764e6e0e8c1af295ee8b11d6313f811464420b60ebc0bb9b47ab7961e176c168250ea66412cdc68a3345a265c8e482078d" },
                { "tr", "ec87b6eb7b9485248418533b34ac3a79c562ec32f65c84a9941a961a9c8acd2af472b8c7ca977d9858a3d59329acb01c7b2c41dc5c00dc5ddb121a5bacb5fe83" },
                { "trs", "d41822c3d42ae0266e205b4db49e22411d204adb64f0d80cda9b0578d0fccaf54c6ca5fe18a6657292d7142d9b63efdc03fad03db09966fcb3fc1732b1cd9120" },
                { "uk", "746199ea3d1e28484da9581cbbf4ec65293eb84cadabc567dbfa583ba83b50a97264a1d5cd028320dd896b81d68b950cdc5b32af1201f6a6ee770668e0fea94d" },
                { "ur", "65fb8b07926f492a473d11e069df730e07a4ca7bca9e695f42bae9b9f61681f0046e40414cd8ed70c2fc4ab4ddf171931e05de12fe716a7a80d0a771d4c49832" },
                { "uz", "c862b5a1a5562b766bca15578f1001b45ee757bca56bdac33b31e9e2f00b4356db48a03a8d932fd37da54636008b4fb672c390f2432cde3df6a47ee1f372edab" },
                { "vi", "f34631f5cfe539a806e684fe51206e8f099506bceb468efec5e7c70d66a5ccd657aeecd80d977eb8a591bcc1cd60fb6b52db7be21d0571e38ca7fc99f18786b9" },
                { "xh", "3bedd9f40a8c87c949ee5a927ddc6a3e320ac5e0e7ad372a1994209953179efb42af784ee6978d5c2f1c51f845badab1eaaa282de21e96c08e84367358c40426" },
                { "zh-CN", "e9d8a70e2dd302b18e116bc60c8ff0638535c6e8137caac26314f1c55039266a5694e78f67810b5080bdc49e7f121d06349577923bd023b02f71a1a3fa93eac0" },
                { "zh-TW", "4f328b07ab35f4bd650561d779f240dff9d991c39e2c3951fb15b052e20a13892f4213bb01e00f310d512cf23ec5ef44224ddddff12fd4588f46567c04f9cd2b" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b6/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "a6a2f16f64f776d603637ab01e6229d45c7b1d7d6d92e81ced175a0080135f94f1667150694538499fbb5ef878b22897daabacdd0257b70b739d0015b1398ced" },
                { "af", "010e054fa905cf99af66c083937ba1e913819dbbcf67ccdfd466f787b3b12bbc77de535afed37e9acb14eaeb0becc26d0bc0dc53a6baea29da3f66d9916a2210" },
                { "an", "aa41ce67655e3e3e9ee6ef7c4eb5a808303a7d8b69fe06c246aa433dc4f0796abda24e28132e151c62f744046c5ec7b743e3c585f0d7f1bfb95acad5ef06c142" },
                { "ar", "3f141cf4e27fa4d45f7cc82816480849e415d437336f0b8971b61ee3f81d4a1cad758a20c8f5e9b36b8d9dedafe4bb2308490266192893753deb528a36af34fb" },
                { "ast", "84d0a013f79f435bd9e5a7999d2971325f3d78a01cc15a8537b1e6c44e1dbd13c445a38617b8f8e610292ce7d3c0e7ba3a456a723a7cf2e71559e5d50375f411" },
                { "az", "737d06bdbe40be1e37cf65074db875257bbe63da599dc28a021e4a230675cdbb171577d60050c89011fbde7310963746901d4e02f1f867883cdb87aee26fdb53" },
                { "be", "1de9370f2c04a913e29b75c60509648bfba25ede899be92ebe109a7f62a0022427d194db0826f7bfdc51ae92ed0441c6d6309e8ebdf3676a6fde52ad7475936d" },
                { "bg", "26f317dc559cb015acb0bc61fae94d66e098f5b2fda0bc97027f3a80b2d6e1678b77ff0297565aac2100c659237753d83b17fe10918b94bb8cfd3d36f892adb2" },
                { "bn", "f622308a637cc10a259628f714129295374487e0c7b7881511ed24b228c383dfc66e7dfaf2e11a64fbd4f22400f2ba3fbefa40c228a4afe15379f5ef1c9c484e" },
                { "br", "5105f6d3cac3be818e7771ff9dada97a6e74d5b89fb0223125155099a8c7de6da9c741bc8f8da2466da60043c4cb00d0628b872a5e96ba76e366e11230d64dba" },
                { "bs", "906135e65ccfd469088057520835fadb6f9ea8d860cb04d2deb4bcd43cacbb2c4316bbb5ceeb4e16fbe92713725bd5625d7d0f9d27150f8b722c1fcb01106407" },
                { "ca", "fba3a4401ad9186f1156e5b22c38e30be69385817ef31d64b04a90972cefd209c4399d45191bab1c5d76b99f55f2d8a8f22558b44070ba49a90485f5130e11ec" },
                { "cak", "fc368ec9ae27c34825477e67c58c117d9a90f56a7dc77cc0a1795e81d7aab7e4dce127442a049c3236e3382a4643c0f3b1c35c628160834b2581d9cd6afef387" },
                { "cs", "95b67019e4bdf82be166eddccc869a63f294dc982de667139b49fb2b2cc7ebc75988411f8e894565c1d03d39a798429b62e78275fdd19598a369f69b39c05dcf" },
                { "cy", "aa0f14cf61caa9844108daf26c851e3dce19c31757eba786afd4d15f27b4af849a7fe33ef5d99d5d0338213a70b88b0ec255ed22caa5c7e5f861a5e8e5f0b982" },
                { "da", "2207af7cf3e54fca0c482dbbe40ab57a1a3a78325584ba1941f21af83ac5f6cd26874940bbf2b3e02c9d212859f27a77f618b511780ed45c593e5716e50dad40" },
                { "de", "218605237d921788da19ffdd2f1dfa5737ca10beadd13f830d3dcc5107cc8934ad018ff6c01bab12f551404c83c0f620da1b6ad90591e6897c9925f5b589f76d" },
                { "dsb", "7c9feb77d38bfb8da432a30b90acfa347d9f36cdd7abcd682c44c4033cf240ad4ae802d146b455e190fc338ad27963cfcf78c0b93239a036e10381bf9fcabd5a" },
                { "el", "117e6a52164e240678df7fe563bcb3380d07d2bf39556bd2a09aeec89bc95327901a5f386df2ca08e469754548896c781c97c5df2d1a15f952f020365f4b5701" },
                { "en-CA", "2ab99333f352b98dd4f05b5071802cfe69ecbf631c5151ce9aac667a6ee0e2d76afdf7e67c01e9be6122c77f27e1869b901cfef2f94e065121c5c605b5e894c4" },
                { "en-GB", "5ecc6112d4a9d984124c1234bfe3ba1b1fb8cb93db3991514665a7b00ea76a679e8fd65742a06b9b7d8fb921e327c6a6f52304502dc22ca9657f5bdbab81ff22" },
                { "en-US", "1fe35b0c509ff96fa5ac7207005cbdbee70870c74c03e9d1cfe69394c2edfc6b805aaf0d8ff5563a0ff8031b88d93b29a5ef5edbba0502b0843bdeb9455afacf" },
                { "eo", "b6a6153b2a0f90accad713a3dd8d429655ce760a07b5b6f68939874d4105e69c60168335ab5f6c6566c1ddaf89757081dcce8435bd82328cbc552e8ad55be261" },
                { "es-AR", "cf18a9d016ab05c18355e8a8adcf9b2ed337b8597a7f4192a073f3de5d5f28cae270a03641ce5a959e79f84b618431523364c37679261ad3b897f0f0c0ebae70" },
                { "es-CL", "7120471e9c635b84f89b2fca9d0211314e2400db29e21c38ba02595b55326e40acb94181bce462e07db5681f0e64eeb6632b7966d93521a1ff9cd6e4113325e7" },
                { "es-ES", "634b1b5fa832d9d900db41756f4d0a4849659d396c030691d1a738949da15f2f12c3f10bb228ad4da64646764931c91b7d57f7e504ff99c53fc07d9497721949" },
                { "es-MX", "49f1ac7af81502c5d4be294740cc73aa9d0ffcac4ea2f58e2915829654a43c9b32c0a855f74366cbf2de5b310ea753177c63c956bbcf7c03b12c51db69cea7dc" },
                { "et", "aeece3571c035b21d4d63b15d709665fc49907617e42012d787c5dd85e9d3079959625d204a67b0875fbc7f66c2d5141bac52cf155a04f9ce75f0eaedd7f1852" },
                { "eu", "62e6a12e11792ae35c07b5b4a6ac8927e7987c6c24e1aad6fb19557c18160d765d4c58bb5e509953c5bd566d220027fbf48e3cd7f4b6dc75b93d7944b82647f0" },
                { "fa", "61a9017b036245af14058a4752287a5d140fd825d8321834dcd11b954a0c439a5cf689401d9118478b6665924af23f56256d93a6336ce68119b51cbbdf8b752b" },
                { "ff", "104e2adc3d3f2bf6819762791742b320794284093129b521434cb1e3753ed65886f24dc4d1d279f7efba493468945ba9b56f7823aa717d82ab7b0126aa7706d4" },
                { "fi", "6ba4fd270e75c215f380f49b80daf9d90e5c34adcd82e5359218d161c4c847892a83e9a698125cd299d06ea2e28f307dda5ab92f85b050e99602dcaf534be095" },
                { "fr", "7accb8c0fdaa1146d75e331a78738b6f2b6e7c4e28446e4b2017b87151806f6a5564e07a4659e7b8eac240ebb68790343456e1eca121f1e3c97f40035b13eeab" },
                { "fur", "5a64d0aa111e4293c36f415003fdbabda23b66996bd21e026126c6dbc6c4da1122f828683131596c23d258b5b2b668d0a8a513fe3bf927be834a0a3abce16253" },
                { "fy-NL", "6143b8ab9c72091eead1bd26b2d92c5ea4f6a37752eab30dbc6526bf51546257cd4736ac67babd5f9bb11580dc6e5258f2aa9440e0ff3b84b83920e7f8b7a547" },
                { "ga-IE", "80d9bab4db87b767c435b642aafe09d78ae88789df208b3a6662e8ff29ade5436ff36ae2ff39c0ac3020b6f4eb4e59844e254ee91971a41729e204d62ef7268c" },
                { "gd", "68066ba0d3c51dff36b2bd6e206bc5d5438c6d11522a06fc432d2b6e2c7a4fa6522f7f9c7762715ee0a6738c8aae9c73b5bd347702ae92e9d4e8b6616234f80f" },
                { "gl", "32ac0c7d6279f3dc0b99e1561f1bebae2e7b62084d94362693962799d6e49c4739847d4f3b3e2c351265e9d99d1b0eb3d361179ad8b13b223cf4bd20419e5d3a" },
                { "gn", "e948d3ffc923dccb2f02c3537cac82398a85abd3607e6b7785997ccd9c548082290a5544e67937fa54e02a1f9811cafa274a7800befee4e89e7cf551a4920afc" },
                { "gu-IN", "17a27c1978552c4368a3749104b0bf293a86e426b5e712f05848f2c83be9c1bb9c72e32380dae9f31e48c701805bacf6e24d8f67e99b6b6774e8f2b1d853d650" },
                { "he", "0be1eca96251c1f44d89f3d1ea693e64d031958e6943fe8a33417d0acb0cef3ef9eb20194b08ccd3dad45d7a11d4ef6d3bf60e547c9d62acc6ba2e891af6d1e0" },
                { "hi-IN", "0d6b71649d214db7bd6dea9a83d113fb29bb83a68d1b5832d968bf2b3dca28251cfeb6363ee158940e42114565d2f7d61847f7495114f2e1f6316c9fdef4aa5e" },
                { "hr", "56b9665eb131a67425e8d38646982ea3eeb012049504b0f3d05ed7ce53000fd27926fbf4bbff967201e1df638eafbc9a13c34cffe47d445ff5a3d78894eb582d" },
                { "hsb", "ab78f29cdfcfb625be6a9d7b04a0e7700d7f20fbd9de20daf66cd3b56013ba14a3db56544611c37c78b2ac399f36a336e200dab26d88ce7fe5f3e6311aa1596f" },
                { "hu", "cfff214521a77e4e13665977d50ec5e8018cc9613b5e07316203f485b76b9cacebe19a83686740c6c30555cc4ce0989e9f31bc75a512c2b1fa78c95ff58c31cf" },
                { "hy-AM", "18dc19f5704f85386a12936260ddea38bf9bbb71304473ccd5e418fa798912b2341be952768ffb2d50e15aad5aa2fba34233deb10ab3790d58bacbad89fb6e48" },
                { "ia", "56140ffd056e26c44b4955218c140f1dcc4a2c8150e7de9d99e8f2576fd2a041a5bba7c840d313b7d70d93f8570358af518636f384aa048d945b1e3098469651" },
                { "id", "e93edf07ad987582d3a7d0738a658538497a7e2fa39d8d81454450ba4bd369b5b13608633ad2115c042c81357657da9f2d36e355c3bfb24b458df4ae76c3258c" },
                { "is", "42124db1bedfd5bfcdf62be5c2918adaa1b0f7d3505b6a62f201f85ad814856aaca61837a3d9e5c53c8f5125283713d895f8561e8c321e343adc26cd9f172282" },
                { "it", "35bdda753389254285d5edd5af36392e15c5538e0ee7611c6704dd6bc7d07ef4ebe5959b002abc1a2d04a19d6c0cbd4b9235c36b6575d07d4b7626684fb9991d" },
                { "ja", "7d986dd926ac1c3aee7e1e45288acff402fac2e9863336aa9ba4bd743f463d0c42aa277a2eef7df8050de6fbf2f8a631b0e955322a314f0f3baca03b218adb4a" },
                { "ka", "016acddd9b76a29309d9e6fdec65f88c37a0d0ffd3cbf6454f3b41cf11ec9f0cbdd74ff3776c8bb02ad076c605f1f27b4a3465e92a9be6a52f7fbbc57036f204" },
                { "kab", "6a60108b2bda1ee85faaa6c2c3f3189559e41e76afbf629ea2d10872aa7a77fef2733d6d45cd35ef500545028d7a76b8ff5c19521557d0530ce0b311ab0e0b52" },
                { "kk", "7045255caf852276e33d976175e830c855523317dd4bf9b62f9de4bbd2e9c15b6586de1615be0d500a0e1b42b53a4d4ac81af11ce3808faaa0eec8004eae0538" },
                { "km", "3fe9ffb2e19c7bf2278112ccf9c46a116b37b5737dfbcbb598d91e613c8d32f6c6a0f6f625b8e16db12e21bb9c535157e3bdd90adf4edd0a78804fcce301fe50" },
                { "kn", "cbe7751cfe9afe718362630d4d5a19ef303807a3853f62dd70600de9f705cc6e35705ad572ab1827b59fa4a55ca45c4c8d3d85cd6fe5a7bcb6201b8111d0ec25" },
                { "ko", "9194d3bed98f16f8f2f8fb710d2a236514030f6a3fa64827b3259687bbb8b5113582f96c1875a6c5ba33c4583bbf1acc8f83d90c5c4f3c78174fe09d4ce59975" },
                { "lij", "97206abbe3e938cf9024819cf963c91eb89334d2f4666ce85f28b0486fbb9976f6f16cfb084661dcd477989ff096d02f4d0e1a82052d129946d26de926d0e32d" },
                { "lt", "07c0e85cd224a835a5fb170ccac2dd690f3f2b307865fa3787c1992ef92b5dc7c7407a2990f7b5ca0fe7a138e65fc7bd2e7094f989ff64327d9adc0ab7e18901" },
                { "lv", "97ae03a2e584c7e08d6036f8ac5116f43e5c699e51c9356e84eecd7b6b27524e6eafaac843a4b9eedd79b3bef21d1f75e592bea66aed7850896fc8011b138545" },
                { "mk", "3393f2285138d69e1f58a8361b0af13a2138980a3bae2de6698a3056af8da854e1808794f6ecf6211aa0c51783bcd580ae3cfdf1364d6efc2d151057609bfdd3" },
                { "mr", "c5574425b0b0dc8687f8381d8e112eb27143a9e57ff74ff0ff32d71380a0db57076bbc417eac16706063695c573a82d7b76734230dfc3a4a8bde033cc92b1785" },
                { "ms", "c0b238235cd4547dca1868d9c092a5335dd37bbb6ec06ea3c78b4ecd0e21aae44348255b35d058cc3bc30fc75d3263813340c3151018d330a8dc9793b09aaeb8" },
                { "my", "be00045071c1b709f00d791789d4e17dcb31b1cc7ef439ca8fc8503e64be09e95510c6f779fd2fc7684cfc4303058bd95daaf2991b2a332e71bbaa021f233ee6" },
                { "nb-NO", "be7a452c47a39f8ef0b31d3a6a01d6e5bbba96d578a01ee06d93df217b79f64c10c7915528dd713b68f91ab93afbcf80263bae620509b15c26060c9ec969dcce" },
                { "ne-NP", "674296b6e0687c83282c783b32c609b1dea5897132926171959cc3fb88d1996051c4ba01dcf38b89f3af25b4c08c652b5889589e8ad98ccb75c1c3c7bf845afc" },
                { "nl", "cd6458c38ef803f9fbcf3a6ef5acd6829b297c06356c97ae62e46e366f3764d19a0f7260c36b75f2bb67d6f5d1cefd118a80cb9ccacef78bb9606323e0d5148d" },
                { "nn-NO", "3b5c15b58e5b124e082834da28d3c2c264506d415665f9e27e17b2b480dac2e9c76a78a85925f5a73a739493d9a89337c188d703ff6a0ce5221e91f830ab914c" },
                { "oc", "117ccd57fba6ba2dbd6539de6c919441a2ce875d2a62101ea555c5e95cc1380104c9581593c9d9185121d962930f0025e7a807676396de4fa8a0c56ef7627005" },
                { "pa-IN", "7c9d62aab9b2e9306b2f62a57f43c0cd376f0e97af354f31750c65d55d325eadc185c987a92a2d79484a938c4dd6a2f8ab70aa14536a3f59d812eb7f97a981f9" },
                { "pl", "213e7f0acb51c51786e32cf2b4f2bc2a5b783220ebd1c73bb010dc0d70f99fe484b1742d2a9fd00d8ba57831a77378f7e7e11c68ac7522b2b53c4ea1eb411b30" },
                { "pt-BR", "31dba6e422dab77f305b0750b91363897b8119c4c1d4ae2d097a77d44aae118253e09c486aafd0d2bd4e76eb255f337bd93448ea24b61ea60c8bb36af4e28ab8" },
                { "pt-PT", "f883d08be4448d24b04ac7ad2297f01461fa2fdef4c7b7cda94e1187881180a9fb593f2e692266a24d2a58f497bd8f47ca5ec66f622910a7a8ec12033e79b8e6" },
                { "rm", "0808a4bff011c0de56efaf36515fd7615099d4f5414d62f69b18dccddac9c2f0de78e4495b477dddfbe508d2fb4cc3ba366ec605367aa0dbb007c4993021aa31" },
                { "ro", "897af5f1e3b18e89001d8f03491a44dac9e4b422c543e048e46d8d04f103c28fc140801906627ae705b159d0bc67f7940f26d4470200c7b0121499ccf82ec971" },
                { "ru", "a2678756541498a1834d07b651d076c6b36b15e19b3a096f52a39d4670decae0a9ccaf8afa75ec4533327a312bc1e3cabc4408ef995e2dc520435e606eef7803" },
                { "sc", "ce1c2443b9b197421dd741e6eb22ccb73eee8aa51bcfb0c35b5d4e90c85f33bdea3e04e26d5a1eccc9605b91dc1de2714e190eda71d52bfdd8105995f1a088d5" },
                { "sco", "1fd4aec3aca3744bd519175e442e4caa019838c1aa454701708b782f6a886a3c2b5471810b55a6580e4d1d44118cfd96b668aa6a8e8594dd3288db8bbc8bba4a" },
                { "si", "c0ed4fc3bca282f75f0918a2fdb7227608c1c106050cabfe34cf88aac27684ebfc96582f35317430a7cd9664f24ec50dd11ea1a4cde289eb1b579e40c1885adc" },
                { "sk", "9323ac331427cc6c55982860ebf855aa383cac8100b6afb435befe58b4c8e8f8b454927e02f13b74b176dc170644003e8f9a63c60dc394f9ac326cde7380fa31" },
                { "sl", "e308dc8c49b2a3bdf13704021d0368728f3e29b52e06cc6e94ca6cbf7640d2ce744ecec8c48b9848f6fd61d94ef54c86669b9524aad86950c7f1424a8703c57c" },
                { "son", "5895043287e5031751d1b0e9f41bd8f1f8520541af137485b92a2ac6d3bb0f69085a4445213014e40aa2f1d34991a779fb34f1be778686c429effd07af64f53f" },
                { "sq", "ef881353e1e4052e84e06a64c3d0f7a8571e3021873db3ef32e6fa84f1919c1a24502e12c76b05370a8a7f1d8fca774c8db5012b334e63b384fda6e57794fb93" },
                { "sr", "33b267d49616b5ac6f431c57462845f10dc0d67410e2b89b5f081710d0b7ab7ba5bb513a9cb723d1f9e9d93f7fae3623ed3277b1857fc308800ba98c69ecb140" },
                { "sv-SE", "9c4a63ffc2c28435108ec9371bcdddf058c053dfcb4e1ad06d6f65df8c991d1f0589bf3782e5f5791d3d9c0232ef3be5f51057f7790b6e16ebd269cce0d68156" },
                { "szl", "6eaf4cf85bfc36e1ffdb2b78a2847793a979afe8d1e461e0cfb70d3c181b172d189e3ccfff70cef08e847f39f030f6c3e6ddca2f9b26e75ea14d9b6745970009" },
                { "ta", "d6a479ecd71a06b8499bbe00fafca38c3556a273b4c7abca7d29371a5d5045ba53aa56967d4d689dc8130d9fbbf844dfb5cd642d4010f9cb64aac013901f1da3" },
                { "te", "e9938a8f7f3b518b5aad3578e68ac1a2826c353c4dff1215bb0ec21899f749e46c3fcb266f5b6151006bad18ea19d39b8f90960afcd4ebf85b896667a12f6c07" },
                { "tg", "844b407d2d32977ab8f63c7b0b0dc5dcc12d44844a62c6c7d6756cfbec1dbf26184acee49e79f07bfbf0bc40cab1e66c284a2ed6b420a661b6ad02c6a19faa9a" },
                { "th", "9d29a487ec3002b663743f453a89626e2a9457a57e965f6c62d35c3984443fd2f21c8e5fb6d90124444600a6047ee005dd40ad58ff69cdf6c3c2085236708ce6" },
                { "tl", "7c3b42f7dd0773d489f564baab9ceee89cde54d91586a54d38c5c591a65e6fa786bc5c0ec940d20dcb30b5c7f137618ac86db291b1b3140145a7504f53c8a934" },
                { "tr", "49985887b86c2d25169fdfc08d4aed282c0ed49a2d8be7b9af0b14295e82731bc523d81cd0048793bddb247d500059ba5cffe3206f806c314a107a5041b097a6" },
                { "trs", "2b0dfd884b0389eb5af9c8b11a5be3da42184d485b2b77a3212dc9f3529b13a15c76158586a1ebc35e9aca98094ce1fe780d64f47a2e85f522a2544bcbd95614" },
                { "uk", "bc31419a8a0ac520646114c0165634bcd3db80bd1f217ee474274d32895a5de7ee21986ef64a35c309f2b9a9a52ab5583582048b2dac65f7bdc66ff364baecee" },
                { "ur", "ba055b1acbadfc2893bef5eae889256ef9dbbd244c53e9badb2aa84ab70d719c8cc5c6b2b8da8c6738bf201821b501babfb6ac3e633815da348759adb1197e84" },
                { "uz", "8e62d6ce778428dfa7deaf6970534550551c25d391b1f90cccecae33c6c6abfed97409e4800628cbe7448d5636ec1fe260700abf9e26d3cd7e161c2d8726a85e" },
                { "vi", "5e630c27896830d976fd6a9ada55d78c4734b99a9afd5e3b76a98676800ae50c1542e09b7a3de56642c676f3c3600b088f5aed1b16c882e2647d9324952fa259" },
                { "xh", "90f9c6c66e3d2efa596cd3d9418fe0fd8c415ed2c6e0d354ec05755222ab697586245152b642dd71d9cc5e72fa0b6b23e6326bbaa9a687620970960380564ef3" },
                { "zh-CN", "a04f564de60bc622b89602706963a13d6bef7ee295b16b2d97a17c2453aa9e7ff04bbf22af062c0e37f0605cebf6ab3f46bba48a4d7b41c9a6c6fae34dcbbb89" },
                { "zh-TW", "f80bdb67003f63b0aec06b1fa96b7e4118bef4bb2a265cc22b0edc42459e6bdd5079f0ab96f82f11b0936d5aba3e8a13d738ea28aeeac686277a4e36f473d291" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
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
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
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


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
