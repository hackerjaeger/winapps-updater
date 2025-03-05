﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "137.0b2";


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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "0919dc770f5175f46512c8c836c2c36719bd0bc52032c9b89d4913e49fdad60a244efb810b186e8d0ee25d8b85070b97767edb0fedcf0a23bf2e3d7e26cac53b" },
                { "af", "967c3bab72ead78627c1cdc9cd33a75ea13dbdf59661eaaaa58bbf71e0644a592cdbc5527f01caa47ef732ae91ea91d7e82de0f5414697ec188cf6807f259ff4" },
                { "an", "f9ab470d80cdd7988b684402166f77d32378ecb08bd0a0230b0d3d09fccd3e70348e24bcfad30ece8395be4f93c786ffdcf5a55c2a74ee9754b146fbe8e622d5" },
                { "ar", "9b5e98400e8809c8ec0f943c98a58ffc2b8f335f6c9f18ca16f983afec867f39600b82c8557f68457670dcad917646dedaf458d3cd8abe16548409bb5b18442b" },
                { "ast", "6ab72ed788943a9941294fa6d9bc562968630e89ac46ad66ec0d57402f657438dcdf042968288dac2ef09a3903303e543d9ace9493c037d890a519d90182bf7a" },
                { "az", "d1e2a8d87845bcdbfa0e49a0e7773771c4b8172d0acb1d9af1692840ff220d461589d9c1ed643469896e27ab46f681489e236c47de8fa35aeecb14495111ca28" },
                { "be", "d96c88203c130213856553c8240c0aed9924106538b6661daa1fa5a0b8fb45ce01f84dc76085c815bcb93ceeb8bde41e6b27cdfe8377e25ba61ed821deedf09e" },
                { "bg", "765b91679e14c767057efb2715c53942efd427f0bb0b82a74c033253e3a3cda661ad14e888a7ebf79bc1c2b50f0f670704bfe9e2d37556fa9b6170fd02b2e557" },
                { "bn", "cadd351d0aeb685c72e8dc14da82ebeb569af7d0c403dda4241817a164ddbc5a845df9bb366f80f24c761527623194511e52241e24ac42a5cf217923f9e6cc44" },
                { "br", "c9feec8f96895d52eee70bc494f4bd5cdb542439ac8237f921f4018d8ed0e465dd0bcf87c30c59dec7764bbe16440c7626fd241ed1156860eff9f896e85ae34c" },
                { "bs", "e88d5ec9bd838aee6a44cccd32a8d2dd919a622d8352b0aba7361e79595baf255c13941b617479d51ce8f26efc482a6044c85292cd6346f7d2df55211c8d9798" },
                { "ca", "febe271e55d72147640fb624306b38f39b1d537c06ccd416457f45e890e167237317ac59d0d646afe4e96dd206dc9044598afb3aee826edd6af64f695eff0970" },
                { "cak", "c64bd3d07bad0ec0616dc10636f7569d8f371d806ad93c50a4126b832369b7f85e5c47b80eaf387c3ef5ab8d8d75c610993700da8a03cc89a568a37b09d478ee" },
                { "cs", "9adf967210b9f7b8ec16306971a19e0d0202f47cb1e30761b84e6e3616e9801446830396ddcab9dbb6733b537f1c97177920a46852083fc8ff0a339fca89f9f5" },
                { "cy", "8cf9c1b7d0d8b61c579d70d3ddeaad24ff919c8845c994d0095094f2db2e87e55503dc2f37f0fa51089c26adfb9f6612a263ff6f17bf1303e7cb613ffcd5cc9a" },
                { "da", "1dfaca88104c8c14e24165f3e648dc0fddabcc8c56b3121c4da995a230e459dee78faf9c63493e1667cab51c2dc2d979db26a07ec60d5cb8f5ca070136e2dc53" },
                { "de", "2e6e0c4c22429e277dd6b017a41aecd30202006b40d43d19f0fed7d1466650fd2868f0f5d6c20a1601475395e5cd5f84ba593bb64b3c31898a78ab1c9ddc3006" },
                { "dsb", "ef0353990cad189dca39c9847fe9aecce35df102281f11efe1afaf79458cd459c6deec07be8429dca6237e7f14da8e7704b53c6f55b46b74fc1fa74034e0c07b" },
                { "el", "a95ecc8c0015181398a7ff9c36f811212dd4fc987a3ee6438be792a227bc4942cddbbee6f43b7ee9e5dd02961a8e822d7f800fb6d1d91c7a9d9b9fedc205fef7" },
                { "en-CA", "6dea35d18191f69141333bf14b1dba4effed2c3f9f4cf28d23380a81c65486350702e7bc68490fc6cf89a56bba461f6c80eec7b80ab662f5c92b1d56ff48f310" },
                { "en-GB", "81d7f85d2e7dd86563dd5026e1e8ef6e798e544ea73707379e93d8b95f6cd5ea6c90374d3de46866bc068e737ab80edafe21caad2ce7230ca4550e01cfe3d4d8" },
                { "en-US", "c23f48b60d488cd7971460b18959d2b9923853a190bd13539b326f26c8d4471ff61580ac119f16a322ebd5cc591daabd8ad52db2befaf096bbbc3c6262692d36" },
                { "eo", "e244844aaa0e26ade0c55aa93dbf0e1a7482a0a00fa0d7732870feaee0cb78bec98fd569287f4abc225ceebee5e67963d296f2310ae8c31db933500d20ec55c8" },
                { "es-AR", "bf1f5de80af6b29ec4adecfb833d57374f0c85bb5b6ecb6e5fb12f257b5be29c2ef898ba9ab05b4022be108622f62f15db271b6947b785c4a93cf65fa520eced" },
                { "es-CL", "9186d0b1ab9091fe751001787c5e1326be6f5b97c84071a1454b75b21d8640a71dedcc63882a1a02a19b7755b38a0091f19fd88a9a3134def85a1fd49093d655" },
                { "es-ES", "a8ea158ee4fc253c235798c8f729518869e7bcd3c217f133c051f3c717142753220c1a386654f5e49f67af010ea659bdfeedf30dae0a63fa863f9c34cf18e81e" },
                { "es-MX", "32c36474977d6451afe71772dd703295c53600b938511812d1d4fca5fd735c5f27f79229aa9e90bc8e8a52444fc40f89067f2b21eae43141d4202290d2ca8125" },
                { "et", "a9ee3e477b129171f55b29f19b82810c5e4f722b135110cb88034ecc40fc8c7677106e52cf5cc799ade7f71a79ecfe8710838bda5e378f23a14f705a5b96e8ca" },
                { "eu", "9973a09cd5d0abf0956dec04e6a7d730d22032ea101d06964a8bb394142ebf4ce8eea33f792479da283933d86d4dfdbbe5b0cc21232075422b9bd101143648ea" },
                { "fa", "817f3e96c91b85c0e74c9d1900c5bde6bf144f873dc9cb24e93d21771a4c040d9f0e7f0d11762160f999fd421a3d4e6c9592a1ea49a0eb95cef04702ec9fc6c8" },
                { "ff", "ca941b8519da5a66119615890e76e41219579b90351496d32e6004536ed3ad1920053e84a7ea97eb782eda21c1874e521ef257291e356f46570daba79f1789e1" },
                { "fi", "33b8a5374871ccdf71084024b446bfee6577885b960a485875eb49e2df53ae5e504a50b9cbfe2ab884479304c53c099c348789b03fa9723a8d7fbf9e3ae1177c" },
                { "fr", "695cb254289ac49f0a80ea84aa3be7f2ee61e3b384aab41b6646aa7fe2729e460821b3ce55901cce53786dd55ce6fd16d29c1766aec887df21abb892fb48acc6" },
                { "fur", "4d4fe91708efa41c4c33d3d5723df522a90c02277f0232237d40829b63142d082249f7c2a92eb67feca2b6adaacee37b9a49eed9d6d3722f4d92c114de736feb" },
                { "fy-NL", "21b09f540e1af4c68c81da3f31bae903fe4f566b891907ae691f52ab8677ff7ab6a82fddce729c1e3ee86f9449ca44a3ed82effbfccbb9452da0cb2260009817" },
                { "ga-IE", "e23cdf7b686e9f474c21835aebb4c36c0035a123e4d4b7f841b58a0c067bd54b3874ec5b527c47883ebd5e07fd774ed817718af579e746e74eb7a764a68957ca" },
                { "gd", "21aec975348fecb0b455debc7edc4d31b6abb04eef774ed7971d128f30ab2ead1f3762757ea78494a9c4dbf5ed8aff3c5e3df59de9f09f4d5a9b0a9ae346dc88" },
                { "gl", "31a2bea837e000021f4433670bf82abd99696665fd880048b4bb2bca8da2ce070a576f4133b1ad4afa399c468c1761b1f06b8eba2911b7ed9e916917c1f8005c" },
                { "gn", "0b6b4653cb2a7153bb295c44431a803fef5665aad8b95b843185e5682c6bc54d20d5ae7eab7e63f6b87a1012bb5e883da186d65e4205868649d2028a521f1234" },
                { "gu-IN", "b32a6c70b10f799632a1282e22aa091f173dc7d15ef1a5d091a25c12f6a46255661bb6f8a996deafb1b8d621a3bf84e1f15043c11b6f0a244344beeae2226ea0" },
                { "he", "515418b766c1ef1c96b60ba1deb62b82aedd12789e0334049288693b73aa76aa4e0a4b136ae8ae0f5c612a5b65ec28ce59a409693b75b95fedb6f13441329f70" },
                { "hi-IN", "d7eda819251753c83ecbb6f638eebe6e1c6a24c32ecd45e23cec04af627ab548c8a0a935a103877d6fffd15efc27f8092da39ca515312acb871035f05f46a7df" },
                { "hr", "c921b433fe606cc71e4fce643525806cff6a777dbba77f866c69a44bd4ee82d4751bd1440b9ae4627caa418a944195d747b9269e8b47d9d77dfad6a0ba050673" },
                { "hsb", "93ca9e52d2a5cb4e3b1ef76812899a365893a44814931bec937ec41dec748a72c4a093a0775f9b7fe8f8943d59e52eb20433fcd35bac532b42d00ef3c301fb29" },
                { "hu", "b7f6617f14fe5149e4dc60c14eb68b2a944353ccf6e56bd6250617cc0ee02d60c1234dfc64359009bb76b7bbf0b28b2dfb49031e98dbae5fdded7770bb0d2b16" },
                { "hy-AM", "b1484b9b1b53f3cdcc8942a36303e4535f817803e10dda4db233f324d7c1a3734038c29d0bfc8a385043e3364dd1753ed9e9ae5ab216a2642eb7eba1a1ba89a1" },
                { "ia", "0c9c327cfee863076e58274fb5423618dc8e51a297748a85a3e503e1a08c8178be3f1082d06be6e1411e413fe9e24d59913a21cd13bdcde27f985dde0aed73cc" },
                { "id", "d87289f2656f70748bfe6187ff5d6e18c1ff7d26ce1b9e1358bb0103ed44a5297f740af881b3e154ef1681d8e671c44e8c01aef773e9999522ef183faf1b162b" },
                { "is", "7757e34d14deb7107eb171c66e739df62d3323908ecbda3b9f695bda57b9e3fb8605dc7cb788efb90cd729edf54fa7d020579a22e3066ee7f68e532838f4bb56" },
                { "it", "58b4b477fa7b9b5bf0c0d28196dadfd19404c1643900af8d8c4580679cfe614baa0c27d9256cc563cdb4f19cbd7bd59e4b8ac5deb2f9f35790b8b371b4d06097" },
                { "ja", "7d300fd33a780a9c049e011814029b28bef538f8a3c38d645ede8911ce21216f4da0ea37709b5f7d5e1d449c9b4c2e7705d32ddcca450791d119b23de5004e0e" },
                { "ka", "79a8d41489b3eee338be5a6aceb135339abdf2b75b1f8256136c4fd53a25b56eaae08c48b015ce5797d0822bde624fc66770f671550cf2e0e60382d856894de0" },
                { "kab", "0dbc1afe617bd63c26b67954d39b2fbadcd27c2400b408aa3a57ade4bfdc6570e3b15a01c35b7e12b6953975143a9fd8c750aeac8fc0f4c01e6a5f175f09c056" },
                { "kk", "2287f09ce0f30b240437044816e63c559361af114e4c024ffff21d1d9679483c0b6cc1a6dac39652a5816090961672dfeef03698a96bf095c3ade97beda8e80f" },
                { "km", "ac5a32a1f2fc9446cb9272246a6f3bf565da867166e8f14c6c15d65c5bb4af6f37b819f71097aef08184f1048557699dfbbed19ab822376e53d663a5e190ce77" },
                { "kn", "f4f40f95ed4be8a28132f07e1cbcb8265a2fc9ae1e7abb3269c041252a594ec70438aed0a1abb226035e906dae6b954e3360517675110c948efbb49300887a90" },
                { "ko", "3390683f486536b71a60e9e2e905e1d68a5314ba527da06fc25b0e617cbb7131997d800f9c28cbc3ba943fc8671adad8384290c8e7d79f4e219cec5cfd622fba" },
                { "lij", "16f46d63617052680d12ba58da63b9508b333413ae5e5b44bc1535e074265bce064419bffeab92607d3a432237a8e5704d6babe36643774c30b82581c37de4e5" },
                { "lt", "4a1205683114700c8fe664d29eef24d2f7a58741b50413549ca7489632d4a53dbdb653dc69b146e3f2875916357fe1ed5ff1c761819f44cafa48d18063fa6e56" },
                { "lv", "7dd83e3ac4ad5fa465f0ea7a460e1f8364a802b307a4476cca282255e16208cf04f953f81eb4a49eb588ab46778e4b2649fa5b32fe2cd6a59a626b6e25087318" },
                { "mk", "aadb63013fac7ed952824ada81cf7f0b0e0892041295d45b7b264ba0f188edfca80abe5a61ae212359b39bdeff073d8199664e8f7b6236499229816add4a514d" },
                { "mr", "2992055b4db005aaab70e324c29667a8d8a529dca3fec2b79b0c5734868d51bf27e7d6b85b1487ec4e85826fbc3827285fb59d28723e84babee5a9e3cb2df083" },
                { "ms", "2a1204fadc9e09e4bf6e9cc932ddf732abd8c5386bf9106cacf9d1f7f849e1bfd4888b787500fed5d4d6199f1cebde529306e6cc7d42bf734bb4ff5f2071c4a7" },
                { "my", "b432ae95f4b8b6e3421b069f998e6142259852d6b8f29621ab07d965e8a68a4616166f897f328820c32ca8b00d6ebb9f25268be2ea11228bf6596c550432b3c6" },
                { "nb-NO", "faee277418147ed1e697376bd617ac11cf08cb8f20dc04201d82f1e3478b7628163ed0cbb68e4c9f9a58636b8041289f721c45cd3e89f5da26ffbf7c80164ab7" },
                { "ne-NP", "e1c3662af49fdc27d9325f0d43082c640841b842b4cf8248f8a59c7fc377d9f693519e09958c93b4296b54c2175f3287549d465bea7c19fe918b4a9af8282f87" },
                { "nl", "f72ba9587679f2ce809d4655a5dbfd82d6894da55cda0e60454367ad122a4b9599d465ad3c64a4cc1f6c276b20d8980bf9d2c6649945647b189c1275c446e879" },
                { "nn-NO", "e4a62abf666e004d456676cb2821a7d21251fcc507b1798980d7dd1278b67fcee304aad6d624c2f3acc7b26273235a8a56ed421f2aba2db5ee0426a0806acb36" },
                { "oc", "de7bf75d3ded8a0d901826c4147ec5d49081360dc74a5cc74fd118ae227879c056ad9cd99d51538aa1f7de7f63c39f87c29507cb66565725b73cd375de1c6927" },
                { "pa-IN", "2c2e2189748401349838ef16610bca762d13c3d32a5c23f9a933cdedcbcc7e4b212b9510adb16876c053c5efc9a46d0f337418a93fe10a03b4b2a6d56f29de48" },
                { "pl", "a8898e3f30a9de13b323fbaa784f68c71a11b0548956afe9b817fe5adc1d6f69c6492f057fc942a66962e777e32449eabf28736fc555ce1127351a0db8348c78" },
                { "pt-BR", "3f74c37dc3c795024e1addc67a20e829841e439c9c7b5aa91b964ae798a0fe7b921a2639013d6f9471d0d6acffa5387d4f299beb59c44c8cd70b33a33b05f438" },
                { "pt-PT", "d149c2556193133778b47fe8752eda265e5383197de648ad69ae8d2de794ad6230b74324caf8b4e28f2fde63ee3fee939385b85bef2343abfa42a50b162f94f0" },
                { "rm", "7c040d54d6cef66513ed5510a25b21d67f202b2ff3014f8e4e44a2709f058ca15bcf3365d06bbabb62e9a795ecdb07f95d315a3ffdb4a3d3e40b79dd4b6f04fa" },
                { "ro", "67f8c518ce226e33808b343b93042ca978b498d42b91c116585d2eb97e61f56e3b0bfe4b41cf657b049508cdb71ae1ffd4047e9a947ea6ba3b7c4d8f0836ab70" },
                { "ru", "d0ea07af3920d3917fce30d3ef01513aae182268a9344a92711244db53c574a9c60aa969085db561f16cf04a3871d2b6cbeae54d5677fbd56f524f8cd4f6e08a" },
                { "sat", "fe5b1c162d41728c9c90b689543a839c7d9d7b08f5a3e21b5019f478c2263c42e8c39886ef585fd0207f60805f7595dc26434568bc09da2b5bcb8883eb836b5c" },
                { "sc", "5cb2513e49410f44ded4ee6f30900efe87598389811425c96a69a17dfff77e1d20443d609f76f74e9edb9e63062a5baf416fe38bc62b5406363d5592ce8d138d" },
                { "sco", "0573d0e5a3cd45ed9dec936c2f6a6be4706681b8697d63e70f6845c1da5e2b79e9dab8e3c0d2e904cf9d6009c7d458854e5e1dee36e424193209c9ed6656afd5" },
                { "si", "0916294503ef6015650069b6696af9f72bd39831407dce04623a4972d46f2e0ff070eec75e7fbd0f744ef507e437604cc197e4f588a60b7a1dcbd46f6ad727ad" },
                { "sk", "7670a12755485e4de16210d93e6ad78216c3c11b6b6ceab93adf9502642b09e07403eea5bc11520d008de6d33cb58f42f1c27cc9e9aded1cdced7d21f6851168" },
                { "skr", "30fedb60ab9c433e665c4c5df142174603ce2dcc6ba18b76dbb08f7c70be691642a141c90bb1f505fcf0645efe1542e94acd9980784fc7a89f13bbfd5f40983a" },
                { "sl", "76eabd69c5e81a268b22d384d538cc5817941d8345e58a95db4bedcfef0b0a69e365e61b3b43501d64637a6776c9c357ae3d94dbb4d653b25bb444e67d6e11f1" },
                { "son", "8ab2a2831cbfb180edb3771922fa4baffbab2edf6eff507acd01c1550f81cf150209038861729e01b2d52ff5a86b75405b30b33f8629779a8e55973e6281f93c" },
                { "sq", "f17f25983ad78aace9b50ddefb117d7535e791792ad1bd1f4c41bcacaa00816f8119d1c5228e6208412ab02ba70680703395a8c5704cd81de5f1e3801e2f0b90" },
                { "sr", "807137850d0a6c3fa432677db4470ddec72b8c90fb333c401e583a36a3fd33961c90677da609452e1d7025489d3ee4987628127f0053e2ef306585e11bacde44" },
                { "sv-SE", "ad8e3f5f36f716ea152284ac77a676b08457637c88c24abbb55fd421b924cd67c37c2ff6bfb5461d38016bed0da9ce4906c9fd2af0778a401073e673c3146e40" },
                { "szl", "e06ce4c84fb77ba8de95c7643da854773a67038f69ddcc83510a9a9a699c47b510e7adb62ddbb247090d7afd40efd232755aeec3c4efbfa00065e1c2093cdc54" },
                { "ta", "a4c4b77da927fd7d42ee5773ec8c99a34265d37f3fe6aea84f2ca98f2da54c6cc92a68f0822eceaf54f045cdc933c6944fb48fdb37b3f49f68a5085f262930db" },
                { "te", "e7b200c32314f559f1267d00ac24d9e988ad8dcf3146061916ea4f2778929a5bb1750127b6528719ac66f4c4ee9e6874dfab7e89ae678174790ef34b5658a820" },
                { "tg", "0d72b5b50e78f02feffae811cf558ea3e73caf705cf6bc885c7756e06b7dd6e2ac34ed1120023256b449b4aec6e939287b77cbee89459554271c90c8f0c59b6c" },
                { "th", "20955cb060357abfe71fd3431aafe8045b4b6ee1d1831dc12542d45e2a8471c3edacfa22c817f12b91a90786f05ca116afb18ef140bfa40a4f7906d53e5e63d1" },
                { "tl", "8c3aefd9ee9ffbc717347ae441642076c95995888f98c921d3f71433e70e9307785140529c441dd9bb4df6f3a7ac9637d938e7c6e34023f725383f22942b9557" },
                { "tr", "51019f1a3a7a360ae4e5e2ee761f90a14b0ef78eed3981117bf4e1e08ad99529bc903dee93dfa0b41f36473ab123742056150e1fbc5e9d750f57fe798ce01596" },
                { "trs", "2211a4a89521c92b71644460d62608a7fb2cc34c21699059071464073315202e3157993b9a57490fe75393c8dcf354c10f01e49e6c253fff186b82425d6d0224" },
                { "uk", "8fb61541f2d0d558f86c539a0c1ac2e4a8d97f4b9634064d4ff95a5f87a71666999be269249c41ad52d9c1e7d54ac1aa95fb05f44f9e10cffa7e60d0cbd49768" },
                { "ur", "0a16c7021941c907d25c44bb19c38b8457f3994c2ad13c72c4393685240dc30f8801cec4f4479037cc35bc35ff9625df78c0c6d01c73a8f8e484606d01f4dea7" },
                { "uz", "44356ac3f02a67806b71469dd8b687d74e0cbdecccfa831250194d0dbdfc1c3327348eac186ed494922103e19910d9a0fd171ea685dea3559d1033dd973f603c" },
                { "vi", "064626f644a258d4786b6667616faed8a83080afde032cf51f900cf7241923ad1a4dd8f606b3687909699d1976e7d4ed19dcddc0b3d5f56af470b38910c1785b" },
                { "xh", "3c5a2d64c185a65b186f92dfa61765387f85f0df5567bf35cc3d781542462b3e7e34a5f5d47e6b8eb10e97c6bab2bd5a6422a2182211bb65ac7d8a180553328c" },
                { "zh-CN", "96356403037db32da13ea109aa3547fe8959dd581f49f6105f7550aa670619b07513f91ed1432c16b953bb610df369a1861ac9331a9a34a4a6e9875fad9327d6" },
                { "zh-TW", "6ed15a90308c54477f635f124dfd954a1ca07faea33a1e062d55bbdad795c3ba6e429b26f2a015c4db569d8360e906b1c94320130ad8badfe06ae7ea5d04595c" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "4237fa3aaff26b362cf238c5fc3c8b6a2518551031d17d2e8a641ab3b04f984cba3fe72968a93a8e624a73629e7e7c2edcc24d200763485fd3ec86594c720ccc" },
                { "af", "e745013ec8f48629fe06aabe68d8a10f4e2211949e7e343246a0e10e5531293fa99e9f7556908da0bae0456efd74ac5d16c153ce4344b09df25ece41e663ef1c" },
                { "an", "22fbc045c0205776301b66df21e5749059ba8bbfd342a6d105d936112d4aed8a0285c6b570bcdc6cfabc3525f09fa3ab14b9f253ffd136921d3d796f9a6ccd92" },
                { "ar", "b14469cfcb0945415ad81040e83f42732a74225600730819321c7838bc01a1d5e51142c294ce06b94cc494971472a5fedfc5313382dc253aa92f1c73d3130cc6" },
                { "ast", "441031747d3a59930087baca328cff9cd7f1fc9a8434a12a2b7faa78cdb9d30182b96befa96abda915bde36f709094e445ab5c8235d375a3a3117674a77245da" },
                { "az", "57acebd143dff88b1af04fe564d986abb7570f351f25cbb9b26975ada45d1049cdf765c31c6145679f8d91dfd4f0585743bc7b7c7d5f7a722bf0335150af816c" },
                { "be", "efad9d3dbef7645bda0c0ed2df7df7f58d517063a3d1e392590ea5c539676921ec13ceaab8b07a50c6c7acf3644a0647575bff6a7f6ab1a7e3964a1c19234aa4" },
                { "bg", "426c352e1d378e34e19dc3789ac085baba7993a2462236489753d533ff2bbe330fc0d89cb7a12e78aa7f9d264119af821d8a597fb28948f36e91976ecb4d5367" },
                { "bn", "b1fefbfed549a81b9b1581286bf9461ea3e36fd9d8e7887d66c4db36452808b69c9299770c99cb39f4d2e2217266794dc4bcfb56a02e0bbb9d2c780719063631" },
                { "br", "9294a16626bc9e7a7d58f242b6964555e470a2621bc5d67b3df174fd9e2681c2efc46259199ba099aa8b188fc822d4a9dec6ce772c316962ee3804e08622bac9" },
                { "bs", "55018e14b6a26da01293889f1c5f4ddb5108c150b92bd15f78f179bf66d77fa069091bf73e25826eea413468d9fb96228b7d23f5c3b8685318742621982f380a" },
                { "ca", "812732320efc4226efb8bce6749b6d1803525c9a7a17183b66093a8e953d32f9dd19cf9a098dfe3d8f829c4b79cf37bcb9a372d2222e407298dfd63abc456622" },
                { "cak", "df82a9d8a9aba4787ff4f7015c1cecdf0a0900205969196dc66796d6904d5e8bb1e4ff68008e781ae03c79e906a05104dbaf97ae6861b123754a599290c88150" },
                { "cs", "a873cf09217d6ff2bc59c7f98a0205e4f79f6814adfde8e690dbadc8086e61ba05bd710bee6cd84715cd3b53ae4709c41eb7284dfe70c323197947e34abbbc49" },
                { "cy", "9c414b16544690c469883a2e4135592072750c75eed757bf0e4d16e7761519d66baad877adc1954434340cf65a1608e544ed3292ea79ff0751574d7571a96fa5" },
                { "da", "671502a6b1aa5f2dff9cf2453cf820c4d6a16a918c50bc45124375ed9764b67db71a8a70674ed6306bc338dde9616773d42fa8a645063dc019ae6a15e708821f" },
                { "de", "477038d48eb8a8c1e5860abb7ab02169718db4770b26ef1db8baac294aaaf331b352ea13784e48086453e351dd56dbf1fcf9a82be611444972a4bc8393b411fe" },
                { "dsb", "95daf1b1257d4fb1fcecdd08fd1b2a05d584f523667dae87e4fec704b0a79814b7c3da7b81135d61d8b57354ba401d5427eeac1e08f9b0ba77343d6db5041d40" },
                { "el", "c5df207ac5022bc69bc2139e824140fc255287b9fde6fcef1a6ef1ec759a2ae28a7c4908395c95414e62bc568bb7e0bfb4784a621a433722f9e3309d6e1fee1b" },
                { "en-CA", "589f1583d64a3e4cad2f09646656415f321476a8576ea54e663bc26ec3f14d98f742bd24a2cb9c0e39eb25a7cd46b82a3585c4075dd31e76b1df923cbea4f00b" },
                { "en-GB", "8606eb5975459b0dc7bebd3d94d6443f956af478428d2c7a1f65dd48f5e56105c340bd89f4d425a9118380e2740540c3df4bba2ea02105e3088d60969c0f1101" },
                { "en-US", "3e636a3df667f7572caa6c317843c3b18448448fde7cad636bf58171920912b6e860e436eb49d0d2b10120abe6e164e461ba744d419b2a9ea78e57cd942c12e7" },
                { "eo", "6a66020a8f479b90bb0939be0f3231c72c731da28e3a106a3a87492bc79b8fb72d3a2f3ee55823e78ed56db21cf2bc7ecf7cfc076435615ed028b7101a720719" },
                { "es-AR", "ba0a60e2c4afe1d115082e69992d09885f42aea91e4f917b194c3eade78e57ba1c09919555aecbc271bdad7d095fc4193f463cdbd2a720b9f2acb9c09ed7d937" },
                { "es-CL", "29c1d4c1c7af01cdcb44b47ee23013d895fe551e6347d01623dc13c99c21502eaf6450a5a84dbd183c1eb2892cd9dc0c8bef597ae25b67053aaabdab43c68085" },
                { "es-ES", "877801b192270993e558b89390ea2a23277343af041cad7c06ef5c7c46f34cd4183747ca9d84bd9e05ce0c4167e3369576045908d5637c9840a7eb69c5323a85" },
                { "es-MX", "08cb8bbc0d01e26989551c0785fb17954acfd1454bdd3df27827920642d5a006fd13f082fcb1757e82953597df009deea1253f438ca5bfd6d0ec2fd89dd49c9e" },
                { "et", "f7526dc34f405b224f7623ce0bb862ff55fd98afebce3f1211353e475efeb936ab53a85668adec04555d916850ee48e15970560dfaef14dc7ec5dafdb908818d" },
                { "eu", "3359393c12dedfc3beca9dcf226df220ff1da1bec973e86cf71ce2e155b1ad836672c774dadc2a8ee175039463e07bb01ad7c70c8816e6636fdd741b7a10d660" },
                { "fa", "9844513ef17f9e7cfa6f62c908b01f37067c747de3a1a40a14e74db4b44b8fa28cf2aefefd5d398e434f00c279feca846ccd2fd1b34d3c9662510a43cef9b847" },
                { "ff", "df98ca3bbbb1db8accc67f580b568b89ba6f0d72dd9a10438881380451314bbf88779e46fbdce513cce5525c26c65bff97f881aad768c54ce5a31654a8c6e1fc" },
                { "fi", "9e5451c0a4a989f851677ce8cd71fe327e6c7bf7e0083bd91e66d5110259c3ac83dab45508e40acfb2868ab2f82e35bd66e6c2e8acff7b910f923be9b76fe167" },
                { "fr", "ef796c3adbc5a0f0a866a9574bc294d8d5e3c9b4c9fa4187dd8ef055943d68459051d3ee641a667d7ca730ec0f94d3831bdc5de46edfec5c124bc4934dc3a5e7" },
                { "fur", "d6b5ab68f9508e6050a6af3018be278a423b7bb5939dcca699284eb644315177eb06cf6d24c3cd2f552a7e08fc49498e76ae33b735af6be0348eef3a80aad5fb" },
                { "fy-NL", "ca4b31d6e87bf101a8e2bcd208da1e701fbb1e497b06e65f6097acf4751b412c0ee3ee0f3a7772f5bbc3d0b9e93ff5682c85598e7a3892399f4e105f9a887e5c" },
                { "ga-IE", "6589a72cdc21814cb236a06940d9f0d11067d0504ab8ef59e8f9ebb1d671d39c6e58b21ea12e93d3ac3f20470de92609f5ebc977e462ac30618a34ba6a664cdd" },
                { "gd", "95acc2b92876df01e37aeed96a163c133f6c277d8eadf78f2085e05fcbedf117c57e64a0f39cd898d64c7e3b0e75d709e31a0f2cd025f3c5602ba83062bed59f" },
                { "gl", "51390f7cc0111a02d688f68f38cb8b1fd13f260ba660d64bfa8e2ec1073bacdf67118102dc705e7019080e3fd11374c4ea3813ac10eb187dfcc05d9fff97b50a" },
                { "gn", "f72089c4f52d445a6a8e13c7108acb7b70303278a85613d4bb36446bbb0635b12703297326b34c73bca98fb6b45ee35e9d4ca106ddd50ade27b03c79c29665eb" },
                { "gu-IN", "d1acd450dc1712a51114f2f9d2da5462fbff02d38334a25c456d7a1e8d73581b63480681b5988babd24046c3aadce81ec07f1dbf71503ef491520c9ed1ae78ab" },
                { "he", "702787e26d7ad06ab53371d159281a29dac55dddffadf9a0f8dfef734a3b6796accf5ab5e15a2899d9e78e4761d0410a028d1789a883acc0205aeb1fc351fef8" },
                { "hi-IN", "c038cb3ce3f938a76c6ceebf7c6443b47decec7bfb5dc86ebfff4f9721bd527069f94675787bc47024a89f5bd64f64c613cf7c9eb0d80a96ffaa23e210d0f404" },
                { "hr", "46422638d4b87d08a903dbfc4979fce45525fc37df1f0f9341cc0408df528159ea909486343397445b67de664b3bc41936384256ed1fe0855276399b72305b05" },
                { "hsb", "4ffbbc53a027d2bcef4d8b780001db4e45c5e096dda1b0cbe11304efabc5e78b64409b7f55accc78591f8e16203236d486d2ce9debc471a7fead16e76b4e017d" },
                { "hu", "de6c80fc84949f2c5f53cae81d122b92240d8e001f4912cd9612956dd37bf75e07daeee4fca3958eadb2d392d37f65b6f9c93cad38905de67ec105f994a500c6" },
                { "hy-AM", "44b6715166288cc5189067bf95463d07b6c74b50f4075b82c24fe5f79516af5aeabb83c6a4ab2fd4553d910f020a86b5866ed94033604f8e9b57ab503d1f4dc2" },
                { "ia", "0c05c9cf28d5bea5d097a34e46fac69c1b499450d6b765341d218185a4c02df17998861dd63b11316b553db96d530c8e39a5dcd9fa331df2c18a48aa0add3a92" },
                { "id", "3ccb2eea3bf3e65294906b3358f0beade88ef328ff2e9c9aa4cc302bd81da86a910189c4d93ebec5b69252d48d69150789c321369d65432d2753a13017f81c1e" },
                { "is", "5f8fdf6ab6c8618ab9d14dd8788dce41a75dba8a18367968862e6d8653bfa39e0ee05f102381be8a113ee6286ea624e02248405fdfe301fc506106ca233b0994" },
                { "it", "d11fcb291681a4fc6be1f3b4db4257c37484595d3bd06b397e83b4ce55ed3b2cc8d15af6468dbd1b9a80e8071c6424d7c6d600e88b69fce0c15b30e538eec7f0" },
                { "ja", "38c4dca332124010d84a3dc1d98e032f3ae82f19c42e41a7fe0b10d8baf5d81a0630fc71c82c711ca0b1d3132e0c0f253b638840bba8e6f62416ee0951ea85ab" },
                { "ka", "6d0abc57b01b744d238aaee3eb2ba21c7149fbfb6af884ac57a83f90e9bfa3e53060352f1e3cd28e7b0ed9508896e9708ee114932037017593929f1e5f6cabbb" },
                { "kab", "d392d7bab3f7db89fbd2598a89fd90543d4c22ca1c27ad90fe2958a57fddec14432ab41e4bbcf204f3f39366b0ec52f47af9e863df441a55ccb9a239dba4eba9" },
                { "kk", "dbd6a67d71b2052e06b336e0975009bd7c048f370952581ee842a260ab32157e7984ce0f2a4805495a0178302b22dbcb0f73ad8f93989e33626d9fbd2fc8a6b3" },
                { "km", "78db79550ffa57a5e0d8d322f3d3f4ef1f376fe41c4cbcdbf25008a480aa02063481fa5cd071356edbf99ad08bf44a0d08c3dfd462d400bc1bb1dde3f429ae7a" },
                { "kn", "c4e4a096eb20d86e1b1c21d8bcee7c0c3893407bd34599ef69b9e987c1f361fb81fad73a42f077f323bd668b10265536c6d831c29877db375d1144ab164621b7" },
                { "ko", "8d2ef7680afe772519bb1a80b9ec128fe72c2f30893bb7cf77ea04a6a76f125fe744a75f53cadfb1bf1bde71769e09623bbcf98bedf586c4fb2053fe86e2d92a" },
                { "lij", "be337b0ec7145e08ae5c773f9886ba32b13bbf71dbce7dfcd183b384d5d74045184ec239b5a959cb6bc4b32f12940ef6da578b2237a652345a68541f688470dc" },
                { "lt", "2a67237253981b7e575cf18d1534d64c1d85931bd870acf0b7bad0a132c751fdece2fe39e1528e739d065f6517b3c1631f4697fb26ca0ffa270d826778e703e0" },
                { "lv", "d02adffbfce348b6332c8ece685aa0cd41bb8360f63c317a8e3d03d366eeaed72d64ecc2727bcb6d92df904b56cdbc80bec2684ad35374e80a01aba12fac3bfb" },
                { "mk", "130bda456a03865f779cce7270bb23bf0d4aacb5868ce2f8b3a6d2c765444ac7c6063f36fdfb5be3cb2b1ddbd78689b0c47e6d76965dbc81eba00cdcec51f76f" },
                { "mr", "57a1e6aee0200044534b59e4b8ada5221d64c8fc30e319684696b3ea3dbd4582a0470fe1eb9333bcc74a6c06be3cded4efff53626a037e3576c8403dc149b931" },
                { "ms", "8be0327a8cbed13ab9568d57667a9c30aa0181ae43bdee9fdaada4d0340c36496cab02e64fdc5cc13815a16111b08a4470ec7bf58511370609484f44ce74a3f9" },
                { "my", "13214574d5aa3509435008db9c24a13ea4ed08999a3ce5769c7ab1f25b11afef2244467443fef466178302b691068f16b96e6a52968c395310f4de1dcec78ea2" },
                { "nb-NO", "53fd0007e201d0fc3372b534810be77df23b7e7e6aa9b98eab950e5c4297eeb56cf8c698fa4c38ca10633fcbd4ecabc8c2fa6dfa479e57880fe19ffd34166cc4" },
                { "ne-NP", "a34d014df7f8b4aefcae65e1d7c87d924c4c0e658e4bc59dee3b4394b4803f1c5e6b5e6b892327d96e7c6ee2f91edc87126b1c8bafad2251ae6e223e9569007c" },
                { "nl", "e686ec90eb8f3d3263d29548e9469a161759eea2f4ee83dfadb576842141952ae817ba2b4e0227a395eef8b96f3e9b461568aafd83cad2808557d357c254d724" },
                { "nn-NO", "55b7d18f7c7c415113b95e0aca6fd672cf092ddd3f0134d06ea38245fc4bb230f3f30a47536aba5ba4df74723df5ed5b9670a9dfa1dabad072715c78b5851b17" },
                { "oc", "87d050eb2ff9517a394dc22405817be7b0bdd89d128c24d7c9949db9a38bab47184c4e2673d7a7a4bceedcb6fa8bd826b7245db4fb2cf9b0859a972c7efcf835" },
                { "pa-IN", "71e520ba14853a93e427a1a885faf4eb31b48725d97db713d2b447c34a16c894dad24b8250f7ef3d73a7478cf43da8550ff2435af81b3d8573a9b36f82062c50" },
                { "pl", "5d355cffceaa465963989fa337633773a7c8b1898772841bc3882cae2eea1543084c03d44c7a0cc69d18e6b0f5aae2cb6be5538d359dced70aad2590ea5aedc3" },
                { "pt-BR", "18ebdf0cd543bb1028c24b2d94114d4be36937758326354cca781dab0777e4b50f4ed263a26c9e976939a2a8e455949e81ade751ec3b92d15185198b72f233f6" },
                { "pt-PT", "49eddcd3e1b9a82f54f004ef423aa48f15ba3011344cd9b0b0fca3f268035b188d7dbf728ffb60042ab9cb105aed861b596f8cbb9b5c82a9a91e5d233196d9cc" },
                { "rm", "1f74cf0f6a3ac57513e6be4733b968828b4cf9f728284147321132f8cd217992b64bf2643b6e1fb80b0d8c6940fb62c834d25ac76e2c8c7a1998ed62b58973b9" },
                { "ro", "53c0fd6399ef21b05a62c38542c4c776eaaaff50456443559f5fa627e6267d985dc57d73e1a97c6b03e19ce91355f790a26d6009b27a56182b50e03727145fff" },
                { "ru", "245de9ae27f35ad6d744972bd924d9fcbd3f18bd5340a1613fd968bc6b2f1198007ac05c214d9f2f3a5fc3201125578838b70ff505af83761caeb64090261438" },
                { "sat", "5c448998811ec32582b733d744f66443027a583a25458ed5ecab2dd5e8a71e19b0770ea0907760d74eba61db22ebc034fddb9d65d9ec142dc8bf758b23b71210" },
                { "sc", "a2c1ed162d29bcb44a946d8cbab6e7fafc755fbc93c6668a1792809b0230a0762232b99e700698267270ff18a840d13b37f899f5e35b2bd7ffa61c007cfc7c07" },
                { "sco", "6c8a911f5c284ba0bb720ebf4693b7e1899513e8202239afc1431a41491ee5f9af735c1edc8b487aa0a7081559404b79a3f2eb3bf51c350d6c16596e61358d66" },
                { "si", "29087246cead436b7b8c17d4f7843e9781dc02cd25cdcebcc02d8d8f46769cf59dfc9c6b5262122fec7c2c30d23495f947a100a0a2f86f180863b36f2ab4e647" },
                { "sk", "5f7fa9c17bd4178162bb4d0d28b58693ff3b6c9e691cc91bfec34ae365d4a781461f298434b8dbb8fd20d975ec5f197eb2576070cfce5bc4f3c4274fa08818b6" },
                { "skr", "7e943b73f3da30f495a4d477b46431fa96c8ce5f3c8af1b6283d2212bd78ec3f3f0d4a972f2bf5689da72ef5b2447ee2e93df5ee8e34aac078a7d4542ba0bb97" },
                { "sl", "ab8b4e60b770ea00786f162f91d1fd2a0e2c6b83c7a8ff955284eaf9a86c2b1a2154c861c215237a104057f7290298dcc9c560759fd5efa51329a1eedf2d653e" },
                { "son", "d3dc9da685cf496bc74f92443de4ebe42fcfd4a1f0da43378e1f829e1112f19242b91baca232afa704c1c6126489d381812d00ffe7455a712bc3c5f031ced36f" },
                { "sq", "1fe2623858c8db53df7685932ed1337f58e72dae28df7e10b47e491ce998b919d9b8c49074c4a53463002a2e0f7cfe36a9d7e59be8245483987d291e4cd6a66d" },
                { "sr", "3024f7a452c4f44ffaff01743cbc760283940564788ff37d6be8fd478fd727537714d85c6d58fad22ec329ba3799e5ceed973a9ee1a64f656f9a4aa61c8fd0e1" },
                { "sv-SE", "bef6ce74a03d5cf7081fd58df9449c7f2e10773f82824f217f313529a5aadc9fa8c3350329ad64242e8982ea2850a29ec4429b3a93386f06428723c69ff91984" },
                { "szl", "c81e94ea1c519d70f860aa94f1802c67b454e5c7ce549e9a461f47700713ea7b4712d722b0dc61bf9e26d7777dc8ef939172cf0a813faac7636ad06679313f71" },
                { "ta", "101f017930b7ba723ac65683d302635d5f7c9b0a1dd6a80bd28771d80915444917489aa84d3b422f9f6d01173b526d7f545a39254ee6827843311e29b4f213e7" },
                { "te", "1b90a3ea0190d5e6d0d3714c0f067635fd2f9e2f039f8426e8b93a396ff27ed6e444a282d3d851761a2aef0d45a6b4b1bd4dc8d958b2692f596d9ad105d5a9c0" },
                { "tg", "844dd01cf6eeeb1df93c4d10abe28ea3655aeea90da75df2d0988e65b7a8441e4023907296d6c55e219893e16a6b86dd809d14f7156c20c848f039179148e0a1" },
                { "th", "56f007943c2f6dfa432e5d1d664848639dee8c1c958c736e8690bf308e0bfe8bcae7c3af8d64803549fef6cb081f192e6ad5a2c9dc249463e77e47b84c86122f" },
                { "tl", "d6aa0a93280af93676aa798d509abe18b3d605a6e01b978d5ac2252028567b2b5b4ebd6ad81f212b9ce0378544e498887e550be93444179125731504a6d3352e" },
                { "tr", "571ec7fbbbe1ed51d88e5bf2664c9bab40e9c9fd37d6d7f16d49b32bbabe6acd2f3a7728fbacc72907282538076844f28d344c9c47c1b8117c23c42af1f0c8de" },
                { "trs", "70f9c37557cc6cbb5e95f8d2d2c2f6f4d051871888f67957fb189b9c4341ed3870201234005b2d582b3c28445fa68bdfcc1a314fc0ec597f636903c9192b5a49" },
                { "uk", "1d8c3cb2a3255bdcf50725f077e0bbacca5deadd53a6c7b2b8d4c5bdffba092884fad50f83e6849b781ba88d14b1a8bf9c91458b9fdbfc94335c2b8f1e716643" },
                { "ur", "5dd3ddf086776db62cb448481e6c23aa3c6116c7c4523bf8bda12d5c1c4eee88ad1ba8ad10b2e28cc6ebfca41607f06cf6d4bd136d2480058a5e45ed28cf6871" },
                { "uz", "fe71fb9542846275b1cf5269a2364bebb64a53c3886d80f4077bee8a24850321f71048d3d92fa615c8877ed9536bee289af2585123fa91abf065fdec7fb3f33f" },
                { "vi", "c1b45e912f219d573fc09245d793af2e5b3757445bb1841722828a14c8b92e6715f70b317537d9807198538eded0f6863e035c32a6b654498156ce0a340d25e5" },
                { "xh", "edd28dd68a761d3e19fa2af3296abcaf53a74c6158da43342c99273bd755fb83f1f83fe409870056f21aba79fe2944bee55f5f1f28614b2d6c01c0e53af89c5a" },
                { "zh-CN", "76b46d9a1b4df5b81e730063ef415c710f38fa9bd8f030dc36085554ec2280a6d3f350e5b96a33252fc9b911e4fd8ec3ee1c691188d64331c243c691ba0375c3" },
                { "zh-TW", "a958f27cf5d527e966ba7efedc01af66e8e2ccad3a31c83e5c62f84bb2d4fb73590fe3fbce1eb7216cf7e306d6d5cc5af4b45cc4e7da2d29f7ac0ce7eedcf02c" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
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
                return versions[^1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
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
            return [.. sums];
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
                    // look for lines with language code and version for 32-bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = [];
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64-bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = [];
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
        /// Determines whether the method searchForNewer() is implemented.
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
